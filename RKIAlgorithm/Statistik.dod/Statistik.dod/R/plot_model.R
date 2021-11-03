

plot_markov_model = function(dod_model, result) {
  
  transitions = unlist(result[,grep("A_", names(result))])
  init_prob = unlist(result[,grep("pi_", names(result))])
  
  par(mar=c(1,1,1,1))
  # Create empty plot
  plot(1:10, type="n", axes=F, xlab="", ylab="")
  
  # First panel
  rect(1.25, 8.325, 9.975, 10.075)
  text(1.1, 9.9, "Each week (t) is in one of", pos=4, font=2)
  text(1.1, 9.65, "two states (s):", pos=4, font=2)
  text(7.5, 9.2, "Endemic state (E): ", pos=2)
  text(7.5, 8.65, "Outbreak state (O): ", pos=2)
  plotrix::draw.circle(x = 8, y=9.2, radius = .45, col="lightgreen")
  plotrix::draw.circle(x = 8, y=8.65, radius = .45, col="tomato")
  
  
  # Second panel
  rect(1.25, 6.025, 9.975, 8.275)
  text(1.1, 8.1, "Probability in 1st week", pos=4, font=2)
  init_p1= round(init_prob[1],2)
  text(1.1, 7.7, as.expression(bquote("Pr(s"[1]*"=E)="*.(init_p1))), pos=4)
  plotrix::draw.circle(x = 4.1, y=7.3, radius = .45, col="lightgreen")
  arrows(4.55, 7.3, x1 = 6, y1 = 7.3, length=0.1, lwd=1, lty=3)
  init_p2= round(init_prob[2],2)
  text(1.1, 6.7, as.expression(bquote("Pr(s"[1]*"=E)="*.(init_p2))), pos=4)
  plotrix::draw.circle(x = 4.1, y=6.3, radius = .45, col="tomato")
  arrows(4.55, 6.3, x1 = 6, y1 = 6.3, length=0.1, lwd=1, lty=3)
  
  
  # Third panel
  rect(1.25, 1.25, 9.975, 5.975)
  text(1.1, 5.8, "State probability in week t", pos=4, font=2)
  text(1.1, 5.55, "depends on week t-1", pos=4, font=2)
  
  ypos = 4.6
  ydiff = 1
  trans_cols = list(c("lightgreen", "lightgreen"),
                    c("lightgreen", "tomato"),
                    c("tomato", "lightgreen"),
                    c("tomato", "tomato"))
  transition_names = list(c("E", "E"), c("O", "E"), c("E", "O"), c("O", "O"))
  for(k in 1:length(trans_cols)) {
    arrows(2.2, ypos, x1 = 3.65, y1 = ypos, length=0.1, lwd=1,lty=3)
    plotrix::draw.circle(x = 4.1, y=ypos, radius = .45, col=trans_cols[[k]][1])
    arrows(4.55, ypos, x1 = 6, y1 = ypos, length=0.1, lwd=1,lty=1)
    plotrix::draw.circle(x = 6.45, y=ypos, radius = .45, col=trans_cols[[k]][2])
    arrows(7, ypos, x1 = 8.45, y1 = ypos, length=0.1, lwd=1,lty=3)
    curr_trans = round(transitions[k], 2)
    text(1.1, ypos+0.4, as.expression(bquote("Pr(s"[t]*"="*.(transition_names[[k]][1])*"|s"["t-1"]*"="*.(transition_names[[k]][2])*")="*.(curr_trans))), pos=4)
    #text(1.1, ypos+0.4, expression(phantom('Pr(s'[t]*'=')*"E"*phantom('|s'[t-1]*'=')*phantom("E")*phantom(')=0.96')), pos=4, col=trans_cols[[k]][1], font=2)
    ypos = ypos-ydiff
    
  }
  
}



plot_glm = function(dod_model, result, surv_ts, past_weeks_not_incuded=26, 
                    years_back=5, supervised=FALSE, set_mar=TRUE, 
                    cols=c("lightgrey", "grey", "blue"),
                    state_cols=c("springgreen", "tomato")) {
  
  # Extract data and make model prediction
  mydata = dod:::prepareData(surv_ts, dod_model, result$timepoint, id = "ts1", 
                             years_back = years_back, past_weeks_not_included_state = past_weeks_not_incuded, 
                             past_weeks_not_included_init = past_weeks_not_incuded)
  
  formula = as.formula(result$formula)
  mydata_0 = mydata
  mydata_0$state = 0
  mydata_1 = mydata
  mydata_1$state = 1
  mydata = rbind(mydata_0, mydata_1)
  mf = model.frame(formula, data=mydata)
  model_offset_all = model.offset(mf)
  y = model.response(mf, type="numeric")
  modelData = model.matrix(as.formula(formula), mydata)
  X = modelData
  pred = matrix(exp(X %*% t(as.matrix(result[,colnames(X)]))), ncol=2)
  
  # plot glm 
  if(set_mar) {
    par(mar=c(2, 2, 3, 0.5) + 0.1)
  }
  yMax = max(c(pred, surv_ts@observed[mydata_0$rtime]))
  yLim = c(-yMax*0.05, yMax*1.15)
  plotSts(surv_ts, results=NULL, range = mydata_0$rtime,
          main="", ylab="No. of cases", set_mar = F, ylim=yLim, cols=cols)
  
  if(supervised) {
    print(dim(pred))
    lines(pred[1:(nrow(pred)-past_weeks_not_incuded),1], col=state_cols[1], lwd=2)
    lines(pred[1:(nrow(pred)-past_weeks_not_incuded),2], col=state_cols[2], lwd=2)
    xpos = (nrow(pred)-past_weeks_not_incuded+1):nrow(pred)
    lines(xpos, pred[xpos,1], col=state_cols[1], lwd=2, lty=3)
    lines(xpos, pred[xpos,2], col=state_cols[2], lwd=2, lty=3)
    lines(c(nrow(pred), nrow(pred)+1), rep(pred[nrow(pred),1],2), col=state_cols[1], lwd=2)
    lines(c(nrow(pred), nrow(pred)+1), rep(pred[nrow(pred),2],2), col=state_cols[2], lwd=2)
    #lines(nrow(pred):(nrow(pred)+2), rep(pred[nrow(pred),2]))
  }
  else {
    lines(pred[,1], col=state_cols[1], lwd=2)
    lines(pred[,2], col=state_cols[2], lwd=2)
  }

  
  legend("top", c("Expected cases:", "", "Endemic", "Outbreak"), ncol=2,
         col=c("#00000000","#00000000", state_cols[1], state_cols[2]), lwd=2, bty="n")
  
}


plot_posterior_result = function(dod_model, result, surv_ts) {
  
  cols = c("lightgreen", "tomato")
  par(mar=c(3, 1, 2, 2) + 0.1)
  
  # plot hidden states
  plot(1:10, xaxt="n", yaxt="n", ylab="", xlab="", type="n")
  text(5.5, 8.5, "Alarm based on Posterior", pos=3, font=2)
  plotrix::draw.circle(x = 9+0.5, y=5, radius = .3, col=ifelse(result$posterior>0.5, cols[2], cols[1]))
  arrows(7.25+0.5, 5, x1 = 8.7+0.5, y1 = 5, length=0.1, lwd=1)
  plotrix::draw.circle(x = 6.95+0.5, y=5, radius = .3, col=ifelse(result[[paste0("posterior_", result$timepoint-1)]]>0.5, cols[2], cols[1]))
  arrows(5.2+0.5, 5, x1 = 6.65+0.5, y1 = 5, length=0.1, lwd=1)
  plotrix::draw.circle(x = 4.9+0.5, y=5, radius = .3, col=ifelse(result[[paste0("posterior_", result$timepoint-2)]]>0.5, cols[2], cols[1]))
  arrows(3.15+0.5, 5, x1 = 4.6+0.5, y1 = 5, length=0.1, lwd=1, lty=3)
  # plot observed data
  arrows(9+0.5, 5+0.3+0.15, x1 = 9+0.5, y1 = 5+1.75, length=0.1, lwd=1)
  text(9+0.5, 5+1.75, surv_ts@observed[result$timepoint,1], pos=3)
  arrows(6.95+0.5, 5+0.3+0.15, x1 = 6.95+0.5, y1 = 5+1.75, length=0.1, lwd=1)
  text(6.95+0.5, 5+1.75, surv_ts@observed[result$timepoint-1,1], pos=3)
  arrows(4.9+0.5, 5+0.3+0.15, x1 = 4.9+0.5, y1 = 5+1.75, length=0.1, lwd=1)
  text(4.9+0.5, 5+1.75, surv_ts@observed[result$timepoint-2,1], pos=3)
  
  # plot description
  text(0.75, 5+1.75+0.75, "No. of cases", pos=4, offset=0)
  text(0.75, 5+0.25, "States", pos=4, offset=0)
  text(0.75, 3.5+0.25, "Time point", pos=4, offset=0)
  text(9+0.5, 3.5, result$timepoint, pos=3, offset=0)
  text(6.95+0.5, 3.5,result$timepoint-1, pos=3, offset=0)
  text(4.9+0.5, 3.5, result$timepoint-2, pos=3, offset=0)
  
  obs_data = surv_ts@observed[(result$timepoint-2):result$timepoint,1]
  # plot posterior descriotion (O)
  obs_text=paste0(paste(rev(obs_data), collapse=","), ", ...")
  obs_text=paste0("=O|", obs_text, "):")
  add_text = as.expression(bquote("Pr(s"["t"]*.(obs_text)))
  text(0.75, 1+0.5, add_text, pos=4, offset=0)
  # plot posterior descriotion (E)
  obs_text=paste0(paste(rev(obs_data), collapse=","), ", ...")
  obs_text=paste0("=E|", obs_text, "):")
  add_text = as.expression(bquote("Pr(s"["t"]*.(obs_text)))
  text(0.75, 2+0.5, add_text, pos=4, offset=0)
  
  
  num = sprintf("%.4f", round(result$posterior, 4))
  hh = as.expression(bquote(bold(.(num))))
  text(9+0.95, 1+0.5, hh , pos=2, offset=0, font=2)
  num = sprintf("%.4f", round(1-round(result$posterior, 4),4))
  hh = as.expression(bquote(.(num)))
  text(9+0.95, 2+0.5, hh , pos=2, offset=0, font=2)
  
  num = sprintf("%.4f", round(result[[paste0("posterior_", result$timepoint-1)]], 4))
  hh = as.expression(bquote(.(num)))
  text(6.95+0.95, 1+0.5, hh , pos=2, offset=0, font=2)
  num = sprintf("%.4f", round(1-round(result[[paste0("posterior_", result$timepoint-1)]], 4),4))
  hh = as.expression(bquote(.(num)))
  text(6.95+0.95, 2+0.5, hh , pos=2, offset=0, font=2)
  
  num = sprintf("%.4f", round(result[[paste0("posterior_", result$timepoint-2)]], 4))
  hh = as.expression(bquote(.(num)))
  text(4.9+0.95, 1+0.5, hh , pos=2, offset=0, font=2)
  num = sprintf("%.4f", round(1-round(result[[paste0("posterior_", result$timepoint-2)]], 4),4))
  hh = as.expression(bquote(.(num)))
  text(4.9+0.95, 2+0.5, hh , pos=2, offset=0, font=2)
  
}


plot_pval_result = function(dod_model, result, surv_ts, past_weeks_not_incuded=26, years_back=5) {
  
  # Extract data and make model prediction
  mydata = dod:::prepareData(surv_ts, dod_model, result$timepoint, id = "ts1", 
                             years_back = years_back, past_weeks_not_included_state = past_weeks_not_incuded, 
                             past_weeks_not_included_init = past_weeks_not_incuded)
  
  par(mar=c(3, 4, 2, 2) + 0.1)
  
  upper = result$observed
  xpos = 0:(round(max(mydata$response)*1.2))
  
  prob=NULL
  if(dod_model@emission@distribution@name=="NegBinom") {
    prob = dnbinom(xpos, mu=result$mu0, size=result$dispersion)
  }
  if(dod_model@emission@distribution@name=="ZINegBinom") {
    prob = dod:::dzinbinom(xpos, mu=result$mu0, size=result$dispersion, pi=result$pi)
  }
  if(dod_model@emission@distribution@name=="Poisson") {
    prob = dpois(xpos, lambda=result$mu0)
  }
  if(dod_model@emission@distribution@name=="ZIPoisson") {
    prob = dod:::dzipois(xpos, lambda=result$mu0, pi=result$pi)
  }
  
  timepoint = paste0("t=", result$timepoint)
  plot(xpos, prob, type="n", ylab=as.expression(bquote("Pr(#cases | s"[.(timepoint)]*"=E)")),
       xlab="No. of cases", ylim=c(0,max(prob*1.35)))
  text(max(xpos/2), max(prob*1.35), "Alarm based\non p-value", pos=1, font=2)
  
  for(i in 1:(length(xpos)-1)) {
    rect(xpos[i], 0, xpos[i+1], prob[i], col="lightgreen")
  }
  abline(v=upper, lty=2)
  axis(3, at=upper, paste0("p-value=", sprintf("%.6f", result$pval)), font=2)
  
}



#' Plot sts object with dod results
#'
#' @title Plot sts object with dod results
#' 
#' @param dod_model An object of class \code{\linkS4class{DODmodel}} which specifies the model parameters and structure
#' @param results Result data frame of the dod function. 
#' @param surv_ts A surveillance time series (sts) object created using the surveillance packages.
#' @param past_weeks_not_incuded Number of past weeks not included for initial glm esteimate.
#' @param years_back Number of past years to include in model fit.
#' 
#' @usage plot_model(dod_model, result, surv_ts, past_weeks_not_incuded=26, years_back=5)
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
plot_model = function(dod_model, result, surv_ts, past_weeks_not_incuded=26, years_back=5) {
  
  layout_mat = matrix(c(1,1,2,2,2,2,2,
                        1,1,3,3,3,4,4), byrow=T, ncol=7)
  layout(layout_mat,
         widths=c(1,1,1), 
         heights=c(1,1))
  plot_markov_model(dod_model, result)
  
  plot_glm(dod_model, result, surv_ts)
  
  plot_posterior_result(dod_model, result, surv_ts)
  
  plot_pval_result(dod_model, result, surv_ts)
  
  dist = dod_model@emission@distribution@name
  mod = dod_model@emission@dod_formula@name
  title(paste0("Summary of result at time point ", result$timepoint, " with model ", 
               dist, "-", mod), outer=T, line=-1)
  
  
}
