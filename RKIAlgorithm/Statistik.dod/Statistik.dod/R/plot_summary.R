
#' Plot summary of COVID-19 outbreak detection
#'
#' @title Plot summary of COVID-19 outbreak detection
#' 
#' @param dod_res List of result data.frames from dod_covid19().
#' @param min_date Minimal date to be plotted.
#' @param plot_avg Logical indicating whether a 7-day average should be added to the plot.
#
#' @usage plot_summary(dod_res, min_date=NULL, plot_avg=FALSE)
#' 
#' @seealso \code{\linkS4class{DODmodel}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export  
plot_summary <- function(dod_res, min_date=NULL, plot_avg=FALSE) {
  
  dod_summary <- do.call("rbind", dod_res)
  
  if(is.null(min_date)) {
    min_date <- min(dod_summary$date)
  }
  
  dod_summary <- dod_summary %>% 
    mutate(alarm=ifelse(cases_above_threshold>0,1,0)) %>% 
    mutate(alarm_group=factor(paste0(alarm_group,""), levels=rev(levels(alarm_group)))) %>% 
    group_by(date, alarm_group) %>% 
    summarize(observed=sum(observed, na.rm=T), 
              cases_above_threshold=sum(cases_above_threshold, na.rm=T),
              cases_below_threshold=sum(cases_below_threshold, na.rm=T),
              nalarms=sum(alarm, na.rm=T)) %>% 
    ungroup()
   
  
  num_alarms <- dod_summary %>% 
    group_by(date, alarm_group) %>% 
    summarize(n=sum(nalarms, na.rm=T)) 
  
  col_names <- rev(levels(num_alarms$alarm_group))
  cols <- c("grey", "lightgreen", colorRampPalette(c("gold", "red"))(length(col_names)-2))
  names(cols) <- col_names
  
  
  frac_counties <- num_alarms %>% 
    group_by(date) %>% 
    summarize(n=sum(n),
              Gesamt=length(dod_res)) %>% 
    as.data.frame()
  
  frac_counties$anteil <- NA
  window_size <- 7
  for(i in window_size:nrow(frac_counties)) {
    frac_counties$anteil[i] = sum(frac_counties$n[(i-window_size+1):i])/
      sum(frac_counties$Gesamt[(i-window_size+1):i])
  }
  
  
  nmax_alarm <-length(dod_res)
  
  p_nalarms <- num_alarms %>% 
    filter(date>=min_date) %>% 
    filter(n!=0) %>% 
    ggplot() +
    geom_bar(aes(x=date, y=n, fill=alarm_group), stat="identity") +
    scale_fill_manual(values=rev(cols)) +
    scale_x_date(date_breaks = "1 month",
                 date_labels = "%b") +
    theme_bw() +
    ylab("No. of alarms") 
  
  if(plot_avg) {
    p_nalarms <- p_nalarms +
      geom_line(data=frac_counties %>% filter(date>=min_date), aes(x=date, y=anteil*nmax_alarm),
                color="#00000075") + 
      scale_y_continuous(sec.axis= 
                           sec_axis(~./nmax_alarm, 
                                    name=paste0("Fraction (last ", window_size," days)")))
  }
 
  
  
  cases_summary <- dod_summary %>% 
    pivot_longer(cols = c(cases_above_threshold, cases_below_threshold)) %>%
    filter(!(name=="cases_above_threshold" & value==0))
  
  p_cases <- cases_summary %>% 
    filter(date>=min_date) %>% 
    ggplot() +
    scale_fill_manual(values=cols) +
    scale_x_date(date_breaks = "1 month",
                 date_labels = "%b") +
    geom_bar(aes(x=date, y=value, fill=alarm_group),stat="identity") +
    theme_bw() +
    ylab("No. of cases")
  
  
  cases_above_threshold <- cases_summary %>% 
    group_by(date) %>% 
    summarize(sum_excess=sum(value[name=="cases_above_threshold"]),
              Gesamt=sum(value)) %>% 
    as.data.frame()
  
  cases_above_threshold$anteil <- NA
  for(i in window_size:nrow(cases_above_threshold)) {
    cases_above_threshold$anteil[i] = 
      sum(cases_above_threshold$sum_excess[(i-window_size+1):i])/
      sum(cases_above_threshold$Gesamt[(i-window_size+1):i])
  }
  
  nmax <- cases_summary %>% 
    group_by(date) %>% 
    summarize(n=sum(value)) %>% 
    pull(n) %>% 
    max()
  
  if(plot_avg) {
    p_cases <- p_cases +
      geom_line(data=cases_above_threshold %>% filter(date>=min_date), aes(x=date, y=anteil*nmax), 
                colour="#00000075") +
      scale_y_continuous(sec.axis= sec_axis(~./nmax, name=paste0("Fraction of excess cases (last ", window_size," days)"))) +
      ylab("No. of cases")
  }
  
  
  
  
  grid.arrange(p_nalarms, p_cases, nrow=2)
  require(gridExtra)
  g <- arrangeGrob(p_nalarms, p_cases, nrow=2, ncol=1)
  g
}
