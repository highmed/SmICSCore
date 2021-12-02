
library(dod);
library(RJSONIO);
library(surveillance);
library(dplyr);
library(lubridate);

cur_args = commandArgs(trailingOnly = TRUE);

setwd(cur_args[1]);

json_data_00 <- fromJSON('./variables_for_r_transfer_script.json');

observed_matrix_00 <- matrix(, nrow = length(json_data_00[['observed']]), ncol = 1);
observed_matrix_00[, 1] <- json_data_00[['observed']];

surveillance_sts_00 <- surveillance::sts(epoch = json_data_00[['epoch']],
                                         freq = json_data_00[['freq']],
                                         observed = observed_matrix_00,
                                         epochAsDate = TRUE);

cur_error = tryCatch({
        dod_res <- dod_covid19(surveillance_sts_00,
                               fit_range = as.numeric(cur_args[2]):as.numeric(cur_args[3]),
                               weeks_back = as.numeric(cur_args[4]));
    },
    error = function(e) {
        cat('Es folgt eine Errormessage', '\n\n');
        cat(as.character(e));
    }
);
cur_error;

if (!TRUE)
{
    write(toJSON(dod_res), './Variables_for_Visualization.json');
} else if (TRUE)
{
    #BEGIN Create file with variables for visualization
    dataframe_for_Visualization_Variables <- data.frame(matrix(nrow = (as.numeric(cur_args[3])-as.numeric(cur_args[2]))+1, ncol = 11));

    colnames(dataframe_for_Visualization_Variables) <- c('Ausbruchswahrscheinlichkeit',
                                                         'p-Value',
                                                         'Zeitstempel',
                                                         'Erregeranzahl',
                                                         'Endemisches Niveau',
                                                         'Epidemisches Niveau',
                                                         'Obergrenze',
                                                         'Faelle unter der Obergrenze',
                                                         'Faelle ueber der Obergrenze',
                                                         'Klassifikation der Alarmfaelle',
                                                         'Algorithmusergebnis enthaelt keine null-Werte');

    if (is.null(cur_error))
    {
        #dataframe_for_Visualization_Variables['Ausbruchswahrscheinlichkeit'] <- dod_res['posterior'];
        #dataframe_for_Visualization_Variables['p-Value'] <- dod_res['pval'];
        dataframe_for_Visualization_Variables['Zeitstempel'] <- format(as.Date(json_data_00[['epoch']][as.numeric(cur_args[3])], origin = '1970-01-01'), '%Y-%m-%d');
    #    dataframe_for_Visualization_Variables['Erregeranzahl'] <- dod_res['observed'];
        #dataframe_for_Visualization_Variables['Endemisches Niveau'] <- dod_res['mu0'];
        #dataframe_for_Visualization_Variables['Epidemisches Niveau'] <- dod_res['mu1'];
        #dataframe_for_Visualization_Variables['Obergrenze'] <- dod_res['upper'];
        #dataframe_for_Visualization_Variables['Faelle unter der Obergrenze'] <- dod_res['cases_below_threshold'];
        #dataframe_for_Visualization_Variables['Faelle ueber der Obergrenze'] <- dod_res['cases_above_threshold'];
        #dataframe_for_Visualization_Variables['Klassifikation der Alarmfaelle'] <- dod_res['alarm_group'];
        dataframe_for_Visualization_Variables['Algorithmusergebnis enthaelt keine null-Werte'] <- FALSE;
    } else
    {
        dataframe_for_Visualization_Variables['Ausbruchswahrscheinlichkeit'] <- dod_res['posterior'];
        dataframe_for_Visualization_Variables['p-Value'] <- dod_res['pval'];
        dataframe_for_Visualization_Variables['Zeitstempel'] <- format(as.Date(dod_res['date'][, 1], origin = '1970-01-01'), '%Y-%m-%d');
        dataframe_for_Visualization_Variables['Erregeranzahl'] <- dod_res['observed'];
        dataframe_for_Visualization_Variables['Endemisches Niveau'] <- dod_res['mu0'];
        dataframe_for_Visualization_Variables['Epidemisches Niveau'] <- dod_res['mu1'];
        dataframe_for_Visualization_Variables['Obergrenze'] <- dod_res['upper'];
        dataframe_for_Visualization_Variables['Faelle unter der Obergrenze'] <- dod_res['cases_below_threshold'];
        dataframe_for_Visualization_Variables['Faelle ueber der Obergrenze'] <- dod_res['cases_above_threshold'];
        dataframe_for_Visualization_Variables['Klassifikation der Alarmfaelle'] <- dod_res['alarm_group'];
        dataframe_for_Visualization_Variables['Algorithmusergebnis enthaelt keine null-Werte'] <- TRUE;
    }

    write(toJSON(dataframe_for_Visualization_Variables), './Variables_for_Visualization.json');
    #END Create file with variables for visualization
}
