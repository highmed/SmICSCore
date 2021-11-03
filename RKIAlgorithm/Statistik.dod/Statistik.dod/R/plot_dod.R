


#' Plot individual time series of COVID-19 outbreak detection
#'
#' @title Plot individual time series of COVID-19 outbreak detection
#' 
#' @param dod_result Result data.frame from dod_covid19().
#' @param show.mean Show estimated mean.
#
#' @usage plot_dod(dod_result, show.mean=FALSE)
#' 
#' @seealso \code{\linkS4class{DODmodel}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
plot_dod <- function(dod_result, show.mean=FALSE, plot_title="") {
  
  
  col_names <- levels(dod_result$alarm_group)
  cols <- c("grey", "lightgreen", colorRampPalette(c("gold", "red"))(length(col_names)-2))
  names(cols) <- col_names
  
  inAlarm <- data.frame(cases=ifelse(is.na(dod_result$cases_above_threshold),
                                     dod_result$observed,
                                     dod_result$cases_above_threshold),
                        alarm_group=dod_result$alarm_group,
                        date=dod_result$date,
                        mu0=dod_result$mu0,
                        upper=dod_result$upper)
  inAlarm <- inAlarm[!paste0(inAlarm$alarm_group, "") %in% col_names[1:2],]
  inExpected <- data.frame(cases=ifelse(is.na(dod_result$cases_below_threshold),
                                        dod_result$observed,
                                        dod_result$cases_below_threshold),
                           alarm_group=ifelse(is.na(dod_result$cases_below_threshold),
                                              "NA", col_names[2]),
                           date=dod_result$date,
                           mu0=dod_result$mu0,
                           upper=dod_result$upper)
  curr_res <- rbind(inAlarm,
                    inExpected)
  curr_res$alarm_group <- factor(paste0(curr_res$alarm_group,""), 
                                 levels=c(rev(names(cols))))
  
  add_rows <- curr_res %>% 
    filter(date==max(date))
  add_rows$date <- add_rows$date+1
  add_rows$cases <- 0
  #add_row$cases_above_threshold <- 0
  #add_row$observed <- 0
  
  curr_res <- rbind(curr_res, add_rows)
   
  
  library(ggplot2)
  p <- ggplot(curr_res) +
    geom_bar(stat="identity", aes(y=cases, x=date, fill=alarm_group)) + 
    scale_fill_manual(values=cols) +
    geom_step(aes(x=date-0.5, y=upper, colour=upper), colour="darkblue", 
              lwd=0.25, linetype="dashed") + 
    scale_x_date(date_breaks = "1 month",
                 date_labels = "%b") +
    theme_bw() +
    ylim(0,max(c(dod_result$observed*1.1, dod_result$upper), na.rm=T)) +
    xlab("Date") + 
    ylab("No. of cases")
  
  if(show.mean) {
    p <- p + 
      geom_step(aes(x=date, y=mu0, colour=mu0), 
                colour="black", lwd=1.2)  
  }
  p <- p + ggtitle(plot_title)
  
  p
}

