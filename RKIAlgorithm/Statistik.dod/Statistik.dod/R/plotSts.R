

## TODO: fill prob area grey if NA 

#' Plot sts object with dod results
#'
#' @title Plot sts object with dod results
#' 
#' @param surv_ts A surveillance time series (sts) object created using the surveillance packages.
#' @param results Result data frame of the dod function. 
#' @param range Time points that should be plotted.
#' @param main Title of the plot.
#' @param set_mar Logical indicating whether to use margins mar=c(4,4,4,4) in plot.
#' @param legend.offset Position of Posterior legend along x-axis.
#' @param legend.offset_y Position of Posterior legend along y-axis.
#' 
#' @usage plotSts(surv_ts, results=NULL, range=NULL, main="", set_mar=TRUE, legend.offset=0, legend.offset_y=0, ...)
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
plotSts = function(mysts, results=NULL, range=NULL, main="", set_mar=TRUE, legend.offset=0, legend.offset_y=0, cols=c("lightgrey", "grey", "blue"), ...) {
  if(set_mar) {
    par(mar=c(4,4,4,4))
  }
  
  if(is.null(range)) {
    range = 1:length(mysts@observed)
  }
  
  mysts@alarm[,1]=FALSE
  plot(mysts[range], main=main, col=cols, legend=F, 
       xlab="Time (weeks)",
       xaxis.tickFreq=list("%m"=atChange,"%Y"=atChange),
       xaxis.labelFreq=list("%Y"=atMedian),xaxis.labelFormat="%Y", 
       outbreak.symbol=list(pch=3, col="red", cex=1, lwd=1), ...)
  ylims = par('usr')[3:4]
  #  lines(1:length(mysts@upperbound), mysts@upperbound-1, type="s", col="blue", lty=2)
  if(!is.null(results)) {
    pos = match(results$timepoint, range)
    obs = mysts@observed[range]
    res_unsup = results
    cols = colorRampPalette(c("white" , "white", #colorRampPalette(c("black", "grey"))(10)[7],
                              "yellow2",  "gold", "orange", "tomato", "red"))(101)
    post = res_unsup$posterior
    post[is.na(post)] = 0
    cols = apply(col2rgb(cols)/255,2,function(x) rgb(x[1], x[2], x[3], alpha=0.75))
    
    for(k in 1:length(pos)) {
      
      j = pos[k]
      observed = obs[j]
      dx.observed <- 0.5
      i <- j#rep(pos, each = 5)
      dx <- rep(dx.observed * c(-1, -1, 1, 1), times = length(observed))
      x.points <- i + dx
      y.points <- as.vector(t(cbind(0, ylims[2], ylims[2], 0)))
      border_col = "black"#colorRampPalette(c("black", cols[round(post[j]*100)+1]))(3)[2]
      #  polygon(x.points+j-1, y.points, col = addalpha("grey", , border = "lightgrey", 
      #          lwd = 1)
      y.points[2:3] = 0.5*min(ylims)
      rect(x.points[1], y.points[2], x.points[3], y.points[1],
           col = cols[round(post[k]*100)+1], border = NA,#cols[round(post[k]*100)+1], 
           lwd = 1)
    }
    library(plotrix)
    color.legend(length(obs)+legend.offset+0.05*length(obs),0.2*max(ylims)+legend.offset_y,
                 length(obs)+legend.offset+0.07*length(obs),0.8*max(ylims)+legend.offset_y,
                 (c(" 0", rep("",24), " 0.25", rep("",24), " 0.5", rep("",24), " 0.75", rep("",24), " 1")),
                 (cols),gradient="y", cex=0.75, align="rb")
    
    
    text(x=length(obs)+legend.offset+0.04*length(obs),y=0.45*max(ylims)+legend.offset_y,labels="Outbreak probability", cex=0.75, srt=90, pos=3)
    #legend("topleft", c("Upper bound"), lwd=1,lty=2,bty="n", col=c("blue"))
    lines(c(-10000,length(mysts@observed)+1), c(0,0))
  }
  
}


 