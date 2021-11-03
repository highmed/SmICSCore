

#' Outbreak detection model for COVID-19
#'
#' @title Detection of COVID-19 outbreaks
#' 
#' @param sts_object A surveillance time series (sts) object created using the surveillance package.
#' @param fit_range Range of timepoints where the model should be applied given as numeric values.
#' @param weeks_back Number of past weeks to consider.
#' @param freq Number of days in a week.
#' @param upper_quantile Quantile of the Poisson distribution that should be used to compute the alarm threshold.
#' @param alarm_breaks Boundaries into which excess cases should be grouped. Default: 0, 1-10, 11-20 and >20.
#' @param alarm_levels Names/labels of the alarm groups.
#' 
#' @usage dod_covid19(sts_object, fit_range, weeks_back=4, freq=7,upper_quantile=0.001,alarm_breaks=c(-Inf,0,10,20,Inf),alarm_levels=paste0(c("0", "1-10", "11-20", ">20"), " cases"))
#' 
#' @seealso \code{\linkS4class{DODmodel}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
dod_covid19 <- function(sts_object, 
                        fit_range, 
                        weeks_back=4, 
                        freq=7,
                        upper_quantile=0.001,
                        alarm_breaks=c(-Inf,0,10,20,Inf),
                        alarm_levels=paste0(c("0", "1-10", "11-20", ">20"), " cases")) {

  if(!sts_object@epochAsDate) {
    stop("Must provide dates for COVID19 models in sts object.")
  }

  dates <- as_date(sts_object@epoch)
  weekday2factor = c("Montag"="1", "Dienstag"="1", 
                     "Mittwoch"="1", "Donnerstag"="1",
                   "Freitag"="1", "Samstag"="2", "Sonntag"="2")
  weekday <- data.frame(day=factor(weekday2factor[weekdays(dates)], 
                                 levels=c("1","2")))
  # create model with Poisson distribution
  dod_covid19 = DODmodel(DODfamily("Poisson"),
                         DODformula("ExtData", 
                                    extdata=weekday, 
                                    freq=freq))
  dod_covid19_no_bckg = DODmodel(DODfamily("Poisson"),
                                 DODformula("ExtData", 
                                            extdata=weekday, 
                                            freq=freq), 
                                 setBckgState = FALSE)

  capture.output(dod_result <- dod(sts_object, dod_covid19, fit_range, 
                      learning_type = "unsupervised", years_back = weeks_back,
                      past_weeks_not_incuded=0, verbose=FALSE))
  which_na <- which(is.na(dod_result$pval))
  error_range <- fit_range[which(is.na(dod_result$pval))]
  if(length(error_range)>0) {
    capture.output(dod_result[which_na,] <- 
      dod(sts_object, dod_covid19_no_bckg, error_range, 
          learning_type = "unsupervised", years_back = weeks_back,
          past_weeks_not_incuded=0, verbose=FALSE))
  }
  dod_result$date <- as_date(sts_object@epoch)[fit_range]
  dod_result$observed <- sts_object@observed[fit_range,1]
  subset_col <- colnames(dod_result) 
  subset_col <- subset_col[subset_col!="error"]
  dod_result <- dod_result[,subset_col]
  dod_result$upper <- upper <- qpois(upper_quantile, lambda=dod_result$mu0, 
                                       lower.tail = FALSE)  
  # create var for excess cases
  dod_result <- dod_result %>% 
    mutate(cases_below_threshold=ifelse(observed<upper, observed, upper),
           cases_above_threshold=ifelse(observed<=upper, 0, observed-upper))
  # create alarm groups
  dod_result$alarm_group <- cut(dod_result$cases_above_threshold, 
                                  breaks = alarm_breaks, 
                                  labels = alarm_levels)
  dod_result$alarm_group <- factor(as.character(dod_result$alarm_group),
                                     levels=c("NA", alarm_levels))
  dod_result
}
