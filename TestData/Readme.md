# Testdata Integration

# Queries (AQL)

## 1. Contact Network

The contact network is calculated from two queries. The first query returns every ward visited from the given in the given period. The second query needs to be executed for every result of the first query and returns for every ward the patient IDs for every patient who was at the same time on the given ward.

```
SELECT m/data[at0001]/items[at0004]/value/value as Beginn, 
    m/data[at0001]/items[at0005]/value/value as Ende, 
    o/items[at0024]/value/defining_code/code_string as Fachabteilung, 
    k/items[at0027]/value/value as StationID 
FROM EHR e 
CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
    CONTAINS ADMIN_ENTRY m[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
        CONTAINS (CLUSTER k[openEHR-EHR-CLUSTER.location.v1] 
        and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0])
WHERE c/name/value='Patientenaufenthalt' 
    and e/ehr_id/value = '<ehrID>' 
    and m/data[at0001]/items[at0004]/value/value <= '<endtime>' 
    and (m/data[at0001]/items[at0005]/value/value >= '<starttime>' 
    or NOT EXISTS m/data[at0001]/items[at0005]/value/value) 
ORDER BY m/data[at0001]/items[at0004]/value/value ASC
```

```
SELECT e/ehr_id/value as PatientID, 
    h/data[at0001]/items[at0004]/value/value as Beginn, 
    h/data[at0001]/items[at0005]/value/value as Ende 
FROM EHR e 
CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
    CONTAINS ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
        CONTAINS (CLUSTER l[openEHR-EHR-CLUSTER.location.v1] 
        and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]) 
WHERE c/name/value='Patientenaufenthalt' 
    and h/data[at0001]/items[at0004]/value/value <= '<Endtime>' 
        and (h/data[at0001]/items[at0004]/value/value >= '<Starttime>' 
        or NOT EXISTS h/data[at0001]/items[at0005]/value/value) 
        and o/items[at0024]/value/defining_code/code_string = '<Fachabteilungsschlüssel>' 
        and l/items[at0027]/value/value = '<Station>' 
        and not e/ehr_id/value = '<ehrID>' 
ORDER BY h/data[at0001]/items[at0004]/value/value ASC
```

## 2. Patient Movements

The patient movements are calculated from two queries. The first query returns for every patient <ins>every</ins> recorded patient stay on different wards within the hospital. The second query needs to be executed for every patient in comination with the returend "FallID" from the first query. It returns the dates for the patient admission and discharge. With that information the patient stay will be divided in the different patient stays for each "FallID".

```
SELECT e/ehr_id/value as PatientID,
    i/items[at0001]/value/value as FallID, 
    h/data[at0001]/items[at0004]/value/value as Beginn, 
    h/data[at0001]/items[at0005]/value/value as Ende, 
    h/data[at0001]/items[at0006]/value/value as Bewegungsart_l, 
    s/items[at0027]/value/value as StationID, 
    s/items[at0029]/value/value as Raum, 
    f/items[at0024]/value/value as Fachabteilung, 
    f/items[at0024]/value/defining_code/code_string as FachabteilungsID 
FROM EHR e 
CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
    CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
    AND ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
        CONTAINS (CLUSTER s[openEHR-EHR-CLUSTER.location.v1] 
        AND CLUSTER f[openEHR-EHR-CLUSTER.organization.v0])) 
WHERE c/name/value = 'Patientenaufenthalt' 
    AND i/items[at0001]/name/value = 'Zugehöriger Versorgungsfall (Kennung)' 
    AND e/ehr_id/value MATCHES { 'ehrID' } 
ORDER BY e/ehr_id/value ASC, h/items[at0004]/value/value ASC
```

```
SELECT p/data[at0001]/items[at0071]/value/value as Beginn, 
    b/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value as Ende 
FROM EHR e 
CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1] 
    CONTAINS (ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.admission.v0] 
    and ADMIN_ENTRY b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
WHERE c/name/value = 'Stationärer Versorgungsfall' 
    and e/ehr_id/value = '<ehrID>' 
    and c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value = '<FallID>'
```

## 3. Labaratory Data

Returns all virologic findings for the given patients.

*Current Limitations: No options for choosing a specific virus*
```
SELECT e/ehr_id/value as PatientID, 
    c/context/start_time/value as Befunddatum, 
    y/items[at0001]/value/value as FallID, 
    a/items[at0001]/value/id as LabordatenID, 
    a/items[at0029]/value/defining_code/code_string as MaterialID, 
    a/items[at0029]/value/value as Material_l, 
    a/items[at0034]/value/value as ZeitpunktProbenentnahme, 
    a/items[at0015]/value/value as ZeitpunktProbeneingang, 
    d/items[at0024]/value/value as Keim_l, 
    d/items[at0024]/value/defining_code/code_string as KeimID, 
    d/items[at0001,'Nachweis']/value/value as Befund, 
    d/items[at0001,'Nachweis']/value/defining_code/code_string as BefundCode, 
    d/items[at0001,'Quantitatives Ergebnis']/value/magnitude as Viruslast, 
    l/data[at0001]/events[at0002]/data[at0003]/items[at0101]/value/value as Befundkommentar 
FROM EHR e 
CONTAINS COMPOSITION c 
    CONTAINS (CLUSTER y[openEHR-EHR-CLUSTER.case_identification.v0] 
    and OBSERVATION l[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1] 
        and CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
            CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]))) 
WHERE c/name/value = 'Virologischer Befund' 
    AND e/ehr_id/value MATCHES { '<ehrID' }
```

## 4. Epidemiological curve

The epidemiological curve is executed by two queries. The first query returns all virologic findings and their related patient IDs for the given virus (in LOINC Code) in the given period. The second query returns the ward where the specimen for the linked finding was taken to aggregate the count of the findings to a specific ward. If two consecutive negative findings can be related to a patient with a positiv finding, this patient will be declared as healed and will be stubtacted from the aggregated data.

*Limitations: The queries currently will not take into account if a patient has two positive consecutive findings of the same virus and handle it as one finding.*

```
SELECT e/ehr_id/value as PatientID, 
    i/items[at0001]/value/value as FallID, 
    d/items[at0001]/value/defining_code/code_string as Flag, 
    d/items[at0024]/value/defining_code/code_string as VirusCode, 
    d/items[at0024]/value/value as Virus, m/items[at0015]/value/value as Datum 
FROM EHR e 
CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] 
    CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
    and OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
        CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
        and CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
            CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]))) 
WHERE c/name/value='Virologischer Befund' 
    and d/items[at0001]/name/value='Nachweis' 
    and d/items[at0024]/value/defining_code/code_string MATCHES {'<PathogenLoincCode>'} 
    and m/items[at0015]/value/value>='<Datum>' 
    and m/items[at0015]/value/value<'<Datum (nächster Tag)>'
```

```
SELECT a/items[at0027]/value/value as Ward, 
    o/items[at0024]/value/defining_code/code_string as Departement 
FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
CONTAINS ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
    CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1] 
    and CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]) 
WHERE c/name/value = 'Patientenaufenthalt' 
    and e/ehr_id/value = '<ehrID>' 
    and u/data[at0001]/items[at0004]/value/value <= '<Datum>' 
    and (u/data[at0001]/items[at0005]/value/value >= '<Datum>' 
    or NOT EXISTS u/data[at0001]/items[at0005]/value/value) 
ORDER BY u/data[at0001]/items[at0004]/value/value ASC
```