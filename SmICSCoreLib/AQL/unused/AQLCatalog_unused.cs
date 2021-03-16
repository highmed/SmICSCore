using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL
{
    class AQLCatalog_unused
    {
        /*
        //Testabfrage eine gefüllten Standardtemplates von Better.care
        public static readonly AQLQuery WEIGHT_TEMPERATURE = new AQLQuery("select a_a/data[at0002]/events[at0003]/data[at0001]/items[at0004]/value, a_b/data[at0002]/events[at0003]/data[at0001]/items[at0004]/value from EHR e contains COMPOSITION a contains (OBSERVATION a_a[openEHR-EHR-OBSERVATION.body_weight.v1] and OBSERVATION a_b[openEHR-EHR-OBSERVATION.body_temperature.v1])", new string[] { "Temperatur", "Gewicht" });

        public static AQLQuery EHR_ID(string ehr_id) { return new AQLQuery($"select e/ehr_id from EHR e as paID"); }*/



        /*Sollte aus dem Darmstadt Katalog die Anfrage 3.1 widerspiegeln, jedoch hab ich das Abfrage falsch verstanden:
            Es soll sich dabei nich um "Kontaktdaten handeln, sondern um Berührungen zwischen den Patienten. Daher auch zwei Patienten IDs 
            */
        /*
        public static readonly AQLQuery CONTACT_ALL_CONTACTS = new AQLQuery("select a_a/context/other_context[at0001]/items[at0003]/value, a_b/data[at0001]/items[at0071]/value, a_c/data[at0001]/items[at0011]/value from EHR e contains COMPOSITION a contains ( COMPOSITION a_a[openEHR-EHR-COMPOSITION.fall.v0] and ADMIN_ENTRY a_b[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY a_c[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0])", new string[] { "Fall-ID", "Datum/Uhrzeit der Aufnahme", "Entlassungsadatum/-Uhrezit" });
        */
        /*Query for all Patient IDs*/
        /*
        public static readonly AQLQuery PATIENT_INFORMATION = new AQLQuery("select e/ehr_id, e/ehr_status/other_detais/item/value from EHR e", new string[] { "PatientenID, Geburtsdatum" });
        */
        /*3.5 Contact_PatientList_TpPsK TEIL I
            * q/data[at0001]/items[at0004]/value => Beginn
            * q/data[at0001]/items[at0005]/value => Ende
            * r/items[at0027]/value => Station
            */
        /*
        public static AQLQuery Contact_PatientList_TpPs_I(string timestamp, string patientList)
        {
            return new AQLQuery($"SELECT e/ehr_id/value, r/items[at0027]/value, q/data[at0001]/items[at0004]/value, q/data[at0001]/items[at0005]/value FROM EHR e CONTAINS COMPOSITION c CONTAINS ADMIN_ENTRY q[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1] where e/ehr_id/value matches '{patientList}' and q/data[at0001]/items[at0005]/value < '{timestamp}'");
        }*/

        /*3.5 Contact_PatientList_TpPsK TEIL II
            */
        /*
        public static AQLQuery Contact_PatientList_TpPs_II(string starttime, string endtime, string stationID)
        {
            return new AQLQuery($"SELECT e/ehr_id/value FROM EHR e CONTAINS COMPOSITION c CONTAINS ADMIN_ENTRY q[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] CONTAINS CLUSTER r[openEHR-EHR-CLUSTER.location.v1] where (q/data[at0001]/items[at0004]/value>'{starttime}' and q/data[at0001]/items[at0004]/value<'{endtime}') or (q/data[at0001]/items[at0004]/value<'{starttime}' and q/data[at0001]/items[at0005]/value<'{starttime}') and r/items[at0027]/value ='{stationID}'");
        }
        */
        /*3.6 Contact_PatientList_TTPs TEIL I
         TODO: Beide AQL Abfragen müssen so ähnlich wie in 3.5 aussehen, mit dem unterschied das ein ENDDATUM abgefragt werden muss*/
        /*public static AQLQuery Contact_PatientList_TTPs_I(string starttime, string endteime, string patientList)
        {
            return new AQLQuery($"");
        }*/
        /*3.6 Contact_PatientList_TTPS TEIL II*/
        /*public static AQLQuery Contact_PatientList_TTPs_II(string starttime, string endteime, string stationID)
        {
            return new AQLQuery($"");
        }*/

        /*4.3 Patient_Information_TTK 
           * TODO: Geburtsdatum aus dem EHR_status ziehen
           * a_a/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
           * a_b/data[at0001]/items[at0011]/value => Entlassungsdatum_uhrzeit
       */
        /*public static AQLQuery Patient_Information_TT(string starttime, string endtime)
        {
            return new AQLQuery($"select e/ehr_id/value as PatientID, a_a/data[at0001]/items[at0071]/value/value as Aufenthaltsbeginn, a_b/data[at0001]/items[at0011]/value/value as Aufenthaltsende from EHR e contains COMPOSITION a contains (ADMIN_ENTRY a_a[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY a_b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) where a_a/data[at0001]/items[at0071]/value/value>'{starttime}' and a_b/data[at0001]/items[at0011]/value/value<'{endtime}'", new string[] { });
        }*/

        /* 4.4 Patient_Information_TTEKS (Teil I)
            * TODO: Aus Zeitpunkt der Probeentnahme müssen die Infizierten Tage berechnet werden
            * Aus Zeitpunkt der Probeentnahme und des Aufnahmedatums aus Teil I muss berechnet werden ob es AufStationInfiziert war
            * Aus Abfrage III  wird ermittelt ob und wann es eine Probe gab mit dem entsprechenden Keim die nicht infiziert war. Das Datum ist dann Heildatum zur Berechnung der Infizierten Tage
            * 
            * a_a/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
            * a_b/data[at0001]/items[at0011]/value => Entlassungsdatum_uhrzeit
            * 
            * starttime: 2018-01-01T12:00:00+00:00
            * endtime: 2018-12-31T12:00:00+00:00
            */
        /*public static AQLQuery Patient_Information_TTES_I(string starttime, string endtime, string erregername)
        {
            return new AQLQuery($"select distinct e/ehr_id/value, a_a/data[at0001]/items[at0071]/value/value, a_b/data[at0001]/items[at0011]/value/value, b_d/items[at0015]/value/value from EHR e contains (COMPOSITION a contains (ADMIN_ENTRY a_a[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY a_b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) and COMPOSITION b contains (CLUSTER b_c[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER b_d[openEHR-EHR-CLUSTER.specimen.v0])) where a_a/data[at0001]/items[at0071]/value/value>'{starttime}' and a_b/data[at0001]/items[at0011]/value/value<'{endtime}' and b_c/items[at0024]/value='Nachweis' and b_c/items[at0001]/value/value='{erregername}'", new string[] { "PatientenID", "Krankenhaustage", "InfizierteTage", "AufStationInfiziert", "Erstinfektion", "EInfStationID", "MaterialID" }
            );
        }*/

        /* 4.4 Patient_Information_TTEKS (Teil II)
            * TODO: Aus Zeitpunkt der Probeentnahme müssen die Infizierten Tage berechnet werden
            * Aus Zeitpunkt der Probeentnahme und des Aufnahmedatums aus Teil I muss berechnet werden ob es AufStationInfiziert war
            * Aus Abfrage III  wird ermittelt ob und wann es eine Probe gab mit dem entsprechenden Keim die nicht infiziert war. Das Datum ist dann Heildatum zur Berechnung der Infizierten Tage
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_a/items[at0087]/value => Einsenderstandort ID
            * a_a/items[at0029]/value => Probenart
            */
        /*public static AQLQuery Patient_Information_TTES_II(string ehr_id, string starttime, string erregername)
        {
            return new AQLQuery($"select a_a/items[at0015]/value/value, a_c/items[at0040]/value/value, a_a/items[at0029]/value/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_c[openEHR-EHR-CLUSTER.location.v1]))) where e/ehr_id='{ehr_id}' and a_a/items[at0015]/value>'{starttime}' and a_b/items[at0024]/value/value='Nachweis' and a_b/items[at0001]/value='{erregername}'", new string[] { }
            );
        }*/

        /* 4.4 Patient_Information_TTEKS (Teil III)
            * TODO: Aus Zeitpunkt der Probeentnahme müssen die Infizierten Tage berechnet werden
            * Aus Zeitpunkt der Probeentnahme und des Aufnahmedatums aus Teil I muss berechnet werden ob es AufStationInfiziert war
            * Aus Abfrage III  wird ermittelt ob und wann es eine Probe gab mit dem entsprechenden Keim die nicht infiziert war. Das Datum ist dann Heildatum zur Berechnung der Infizierten Tage
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            */
        /*public static AQLQuery Patient_Information_TTES_III(string ehr_id, string starttime, string erregername)
        {
            return new AQLQuery($"select a_a/items[at0015]/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where e/ehr_id='{ehr_id}' and a_a/items[at0015]/value>'{starttime}' and a_b/items[at0024]/value='kein Nachweis' and a_b/items[at0001]/value='{erregername}'", new string[] { }
            );
        }*/

        /* 4.5 Patient_Erkrankt_TTEK
            * TODO: Ist 'Nachweis' wirklich richtig in der Abfrage
            */
        /*public static AQLQuery Patient_Erkrankt_TTE(string starttime, string endtime, string erregername)
        {
            return new AQLQuery($"select distinct e/ehr_id as PatientID from EHR e contains COMPOSITION a contains (OBSERVATION a_a[openEHR-EHR-ADMIN_ENTRY.admission.v0] and Observation a_b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0] and CLUSTER a_c[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where a_a/data[at0001]/items[at0071]/value>'{starttime}' and a_b/data[at0001]/items[at0011]/value<'{endtime}' and a_c/items[at0024]/value='Nachweis and a_c/items[at0001]/value='{erregername}'", new string[] { });
        }*/
        /* 4.6 Patient_Ersterkrankung_PK
            * a_a/items[at0001]/value => Laborprobenidentifikator
            * a_a/items[at0034]/value => Zeitpunkt_des_Probeneingangs
            * a_a/items[at0029]/value => Probenart
            * a_b/items[at0001]/value => Analyt_Resultat
            */
        /*public static AQLQuery Patient_Ersterkrankung_P(string patientenID)
        {
            return new AQLQuery($"select a_a/items[at0001]/value/id, a_a/items[at0034]/value/vlaue, a_a/items[at0029]/value/value, a_b/items[at0001]/value/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_c[openEHR-EHR-CLUSTER.anatomical_location.v1]) where e/ehr_id='{patientenID}' and a_b/items[at0024]/value='Nachweis' order by a_a/items[at0034]/value/value", new string[] { "LabordatenID", "Eingangsdatum", "MaterialID", "ErregerID" });
        }*/

        /* 4.7 Patient_Ersterkrankung_EPK
            * a_a/items[at0001]/value => Laborprobenidentifikator
            * a_a/items[at0034]/value => Zeitpunkt_des_Probeneingangs
            * a_a/items[at0029]/value => Probenart
            * a_b/items[at0001]/value => Analyt_Resultat
            */
        /*public static AQLQuery Patient_Ersterkrankung_EP(string erregername, string patientenID)
        {
            return new AQLQuery($"select a_a/items[at0001]/value/id as LabordatenID, a_a/items[at0034]/value/value as Eingangsdatum, a_a/items[at0029]/value/value as MaterialID, a_b/items[at0001]/value/value as ErregerID from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_c[openEHR-EHR-CLUSTER.anatomical_location.v1]) where e/ehr_id='{patientenID}' and a_b/items[at0001]/value='{erregername}' and a_b/items[at0024]/value='Nachweis' order by a_a/items[at0034]/value/value");
        }*/

        /* 4.8 Patient_Ersterkrankung_TTEK
            * TODO: Klären ob Entnahme- oder Befunddatum?
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            */
        /*public static AQLQuery Patient_Ersterkrankung_TTE(string starttime, string endtime, string erregername)
        {
            return new AQLQuery($"select e/ehr_id/value, a_a/items[at0015]/value/value from EHR e contains COMPOSITION a contains(CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where a_a/items[at0015]/value>'{starttime}' and a_a/items[at0015]/value>'{endtime}' and a_b/items[at0001]/value='{erregername}' and a_b/items[at0024]/value='Nachweis'", new string[] { "PatientenID", "Datum" });
        }*/

        /* 4.13 Patient_InKrankenhaus_TTK
            * 
            */
        /*public static AQLQuery Patient_InKrankenhaus_TT(string starttime, string endtime)
        {
            return new AQLQuery($"select e/ehr_id/value as PatientID from EHR e contains COMPOSITION a contains (ADMIN_ENTRY a_a[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY a_b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) where a_a/data[at0001]/items[at0071]/value>'{starttime}' and a_b/data[at0001]/items[at0011]/value<'{endtime}'");
        }*/

        /* 4.14 Patient_Nosokomial_EPsKD
            * TODO: Der dezimalwert der zu der Frage im Katalog gehört muss vorgeschachtelt eingegeben werden. Aus der differenz der 2 Zeitpunkt muss dann eine Wahrheitsüberprüfung gegen die Dezimalzeit gebaut werden. Diese gibt an ob Nosokomial oder nicht. 
            * Zusätzlich muss die patientList mit "MATCHES" in die Abfrage eingebaut werden
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_c/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
            */
        /*public static AQLQuery Patient_Nosokomial_EPsD(string erregername, string patientList)
        {
            return new AQLQuery($"select e/ehr_id, a_a/items[at0015]/value, a_c/data[at0001]/items[at0071]/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and ADMIN_ENTRY a_c[openEHR-EHR-ADMIN_ENTRY.admission.v0]) where a_b/items[at0024]/value='{erregername}' and a_b/items[at0001]/value='Nachweis' order by a_a/items[at0015]/value/value", new string[] { "PatientID", "IsNosokomial" });
        }*/

        /* 4.15 Patient_Nosokomial_EPsKSD
            * TODO: Der dezimalwert der zu der Frage im Katalog gehört muss vorgeschachtelt eingegeben werden. Aus der differenz der 2 Zeitpunkt muss dann eine Wahrheitsüberprüfung gegen die Dezimalzeit gebaut werden. Diese gibt an ob Nosokomial oder nicht. 
            * Zusätzlich muss die patientList mit "MATCHES" in die Abfrage eingebaut werden
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_c/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
            */
        /*public static AQLQuery Patient_Nosokomial_EPsSD(string erregername, string patientList, string stationsID)
        {
            return new AQLQuery($"select e/ehr_id/value, a_a/items[at0015]/value/value, a_c/data[at0001]/items[at0071]/value/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and ADMIN_ENTRY a_c[openEHR-EHR-ADMIN_ENTRY.admission.v0]) where a_b/items[at0024]/value='{erregername}' and a_b/items[at0001]/value='Nachweis' and a_a/items[at0087]/value='{stationsID}' and e/ehr_id matches '{patientList}' order by a_a/items[at0015]/value/value", new string[] { "PatientID", "IsNosokomial" });
        }*/

        /*4.16 Patient_AnzahlNosokomial_EsKD
            * TODO: Anzahl der Befunde auf die Erstbefunde reduzieren. Berechnen ob Nosokimal. Patienten mit Nosokomial = True zählen!
            * Eventuell anpassen auf kommaseparierte Liste der erreger und einbringen von matches
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_c/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
            * a_a/items[at0001]/value => Analyt_Resultat
            */
        /*public static AQLQuery Patient_AnzahlNosokomial_EsD(string erregernameList)
        {
            return new AQLQuery($"select e/ehr_id/value, a_a/items[at0015]/value/Value, a_c/data[at0001]/items[at0071]/value/value, a_a/items[at0001]/value/value from EHR e contains COMPOSITION a contains(CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and ADMIN_ENTRY a_c[openEHR-EHR-ADMIN_ENTRY.admission.v0]) where a_a/items[at0001]/value matches '{erregernameList}' and a_b/items[at0001]/value='Nachweis' order by a_a/items[at0015]/value/value order by a_a/items[at0015]/value", new string[] { "ErregerID", "Anzahl" });
        }*/

        /*4.17 Patient_AnzahlNosokomial_EsSsKD
            * TODO: Anzahl der Befunde auf die Erstbefunde reduzieren. Berechnen ob Nosokimal. Patienten mit Nosokomial = True zählen!
            * Eventuell anpassen auf kommaseparierte Liste der erreger und einbringen von matches
            * Ebenso anpassen der StationsID auf StationsListe und matches
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_c/data[at0001]/items[at0071]/value => Datum_Uhrzeit_der_Aufnahme
            * a_a/items[at0087]/value => Probeentnahmestelle
            */
        /*public static AQLQuery Patient_AnzahlNosokomial_EsSsD(string erregernameList, string stationList)
        {
            return new AQLQuery($"select e/ehr_id/value, a_a/items[at0015]/value/value, a_c/data[at0001]/items[at0071]/value/value, a_a/items[at0087]/value/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and ADMIN_ENTRY a_c[openEHR-EHR-ADMIN_ENTRY.admission.v0]) where a_a/items[at0001]/value matches '{erregernameList}' and a_b/items[at0001]/value='Nachweis' and a_a/items[at0087]/value matches '{stationList}' order by a_a/items[at0015]/value/value", new string[] { "ErregerID", "StationID" });
        }*/

        /*5.6 Labor_Untersuchungen_TTEPsK
            * TODO: einbringen von erregerliste und matches
            *
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_b/items[at0024]/value => Art_des_Präparats  (Nachweis)
            */
        /*public static AQLQuery Labor_Untersuchung_TTEP(string starttime, string endtime, string erregername, string patientList)
        {
            return new AQLQuery($"select e/ehr_id, a_a/items[at0015]/value, a_b/items[at0024]/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where e/ehr_id matches '{patientList}' and a_a/items[at0015]/value>'{starttime}' and a_a/items[at0015]/value<'{endtime}' and a_a/items[at0001]/value='{erregername}'", new string[] { "PatientenID", "Tag", "Ergebnis" });
        }*/

        /* 5.7 Labor_PositiveUntersuchungen_TTK
            * HINT: Count nich direkt einbringbar, weil count + normales select nicht möglich laut EhrExplorer. Zustäzlich unsicher ob nicht eine Group By Funkiton dafür nötig ist
            * 
            * a_a/items[at0001]/value => Analyt_Resultat   (Erregername o. Keimname) 
            */
        /*public static AQLQuery Labor_PositiveUntersuchung_TT(string starttime, string endtime)
        {
            return new AQLQuery($"select a_a/items[at0001]/value from EHR e contains COMPOSITION a contains(CLUSTER a_a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_b[openEHR-EHR-CLUSTER.specimen.v0]) where a_b/items[at0015]/value>'{starttime}' and a_b/items[at0015]/value>'{endtime}' and a_a/items[at0001]/value='Nachweis'", new string[] { "Erregername" });
        }*/

        /* 5.8 Labor_PositiveUntersuchungen_TTK_cs
            * copy_strained Information kommt eigentlich aus einem der anderen Algortihmik Prozesse, dieso Info kann also nicht direkt gegeben werden. Daher noch keine weitere Entwicklung für 5.8 möglich
            */

        /* 5.9 Labor_PositiveUntersuchunge_TTEK
            * HINT: Count nich direkt einbringbar, weil count + normales select nicht möglich laut EhrExplorer. Zustäzlich unsicher ob nicht eine Group By Funkiton dafür nötig ist
            * TODO: Hier nur count als direkte Rückgabe, dafür kann der Erregername direkt aus den Parametern übernommen werden
            * 
            * a_a/items[at0001]/value => Analyt_Resultat   (Erregername o. Keimname) 
            */
        /*public static AQLQuery Labor_PositiveUntersuchung_TTE(string starttime, string endtime, string erregername)
        {
            return new AQLQuery($"select count(a_a/items[at0001]/value) from EHR e contains COMPOSITION a contains(CLUSTER a_a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_b[openEHR-EHR-CLUSTER.specimen.v0]) where a_b/items[at0015]/value>'{starttime}' and a_b/items[at0015]/value>'{endtime}' and a_a/items[at0001]/value='Nachweis' and a_a/items[at0001]/value='{erregername}'", new string[] { "Erregername", "Anzahl" });
        }*/

        /* 5.10 Labor_PositiveUntersuchungen_TTEK_cs
            * copy_strained Information kommt eigentlich aus einem der anderen Algortihmik Prozesse, dieso Info kann also nicht direkt gegeben werden. Daher noch keine weitere Entwicklung für 5.10 möglich
            */

        /* 5.11 Labor_AnzahlUntersuchungen_TTEsK
            * TODO: matches
            * 
            */
        /*public static AQLQuery Labor_AnzahlUntersuchungen_TTEs(string starttime, string endtime, string erregernamelist)
        {
            return new AQLQuery($"select a_a/items[at0001]/value from EHR e contains COMPOSITION a contains(CLUSTER a_a[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] and CLUSTER a_b[openEHR-EHR-CLUSTER.specimen.v0]) where a_b/items[at0015]/value>'{starttime}' and a_b/items[at0015]/value>'{endtime}' and a_a/items[at0001]/value matches '{erregernamelist}'", new string[] { "Erregername" });
        }*/

        #region Dev
        /*6.1 Dev_Ansteckungspotential_TTEPP
            */
        /*public static AQLQuery Dev_Ansteckungspotential_TTEPP(string starttime, string endtime, string erregernamelist, string patientID_A, string patientID_B)
        {
            return new AQLQuery($"", new string[] { });
        }*/
        /* 6.2 Dev_AnzahlPatienten_TTK
            * 
            */
        /*public static AQLQuery Dev_AnzahlPatienten_TT(string starttime, string endtime)
        {
            return new AQLQuery($"select count(e/ehr_id) as Anzahl from EHR e contains COMPOSITION a contains (ADMIN_ENTRY a_a[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY a_b[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) where a_a/data[at0001]/items[at0071]/value>'{starttime}' and a_b/data[at0001]/items[at0011]/value<'{endtime}'");
        }*/

        /* 6.3 Dev_IstPatientKrank_TEPs
            * TODO: Filtern des ersten Infektionsdatums falls Infektions vorliegt und Ersterkrankung befüllen, sonst NULL befüllen
            * 
            * a_a/items[at0015]/value => Zeitpunkt_der_Probenentnahme
            * a_b/items[at0024]/value => Art_des_Präparats  (Nachweis)
            */
        /*public static AQLQuery Dev_IstPatientKrank_TEPs(string endtime, string erregername, string patientList)
        {
            return new AQLQuery($"select e/ehr_id, a_a/items[at0015]/value, a_b/items[at0024]/value from EHR e contains COMPOSITION a contains (CLUSTER a_a[openEHR-EHR-CLUSTER.specimen.v0] and CLUSTER a_b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) where a_a/items[at0015]/value<'{endtime} and a_b/items[at0001]/value='{erregername}' and e/ehr_id matches '{patientList} order by a_a/items[at0015]/value'", new string[] { "PatientID", "Ersterkrankung" });
        }*/

        /*6.4 Dev_DailyCaseCount_T*/
        /*public static AQLQuery Dev_DailyCaseCount_T(string date, string boolValue)
        {
            return new AQLQuery($"SELECT count(c/uid/value) as TagesaktuelleFallzahl FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report.v1] CONTAINS EVALUATION v[openEHR-EHR-EVALUATION.flag_pathogen.v0] WHERE c/name/value='Kennzeichnung Erregernachweis SARS-CoV-2' and v/data[at0001]/items[at0005]/value/value={boolValue} and v/data[at0001]/items[at0015]/value/value<='{date}'");
        }*/
        #endregion

        /*7.5 Base_Erreger*
       TODO: Erregernamen liegen auch in Strukturierter Liste vor. Die bisherige Anfragen fragt nur alle ab die eingetragen sind. Daher muss die Query evtl. nochmal überarbeitet werden 
       */
        /*public static AQLQuery Base_Erreger_Mibi()
        {
            return new AQLQuery($"SELECT distinct s/items[at0024,'Virus']/value/defining_code/code_string as BEZK FROM EHR e CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1] CONTAINS OBSERVATION d[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] CONTAINS CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] WHERE c/name/value='Virologischer Befund' and EXISTS s/items[at0024,'Virus']/value/defining_code/code_string");
        }*/


        /*7.8 Base_Material_Mibi
        */
        /*public static AQLQuery Base_Material_Mibi()
        {
            return new AQLQuery($"SELECT distinct j/items[at0029]/value/defining_code/code_string as BEZK FROM EHR e CONTAINS Composition c[openEHR-EHR-COMPOSITION.report-result.v1] Contains CLUSTER j[openEHR-EHR-CLUSTER.specimen.v0] WHERE EXISTS j/items[at0029]/value/defining_code");
        }*/
    }
}
