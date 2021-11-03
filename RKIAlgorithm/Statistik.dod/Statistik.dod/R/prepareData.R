setGeneric("extractModelData", function(survts, model_struct, 
                                        time_point_to_consider, 
                                        years_back) 
  standardGeneric("extractModelData"))



setMethod("extractModelData", signature = c("sts", 
                                            "FarringtonNoufaily", 
                                            "numeric", "numeric"),
          function(survts, model_struct, time_point_to_consider, years_back) {
            freq = model_struct@freq
            allTimePoints = rev(seq(time_point_to_consider, length=years_back*freq+1+model_struct@w, by=-1))
            allTimePoints = allTimePoints[allTimePoints>0]
            
            observed = survts@observed[,1]
            state = survts@state[,1]
            epochAsDate = survts@epochAsDate
            if (epochAsDate) {
              vectorOfDates <- as.Date(survts@epoch, origin="1970-01-01")
            } else {
              vectorOfDates <- seq_len(length(observed))
            }
            dayToConsider <- vectorOfDates[time_point_to_consider]
            freq <- survts@freq
            if (epochAsDate) {
              epochStr <- switch( as.character(freq), "12" = "month","52" =    "week",
                                  "365" = "day")
            } else {
              epochStr <- "none"
              allTimePoints = allTimePoints[-1]
            }
            population <- surveillance::population(survts)
            
            # Create data for Farrington GLM
            modelData = surveillance:::algo.farrington.data.glm(
              dayToConsider, years_back, freq, epochAsDate,
              epochStr, vectorOfDates, model_struct@w, model_struct@noPeriods,
              observed, population, FALSE, pastWeeksNotIncluded=0, 
              time_point_to_consider)[,1:4]
            
            modelData$rtime = allTimePoints-1
            modelData$true_state = survts@state[modelData$rtime,1]
            if (!epochAsDate) {
              modelData$wtime = 0:(nrow(modelData)-1)
            }
            
            currTimePointData = data.frame(response = survts@observed[time_point_to_consider,1],
                                           wtime = modelData$wtime[nrow(modelData)]+1,
                                           population = survts@populationFrac[time_point_to_consider,1],
                                           true_state = survts@state[time_point_to_consider,1],
                                           seasgroups = model_struct@noPeriods,
                                           rtime=time_point_to_consider)
            modelData = rbind(modelData, currTimePointData)
            
            modelData
          })


setMethod("extractModelData", signature = c("sts", 
                                            "Harmonic", 
                                            "numeric", "numeric"),
          function(survts, model_struct, time_point_to_consider, years_back) {
            freq = model_struct@freq
            allTimePoints = rev(seq(time_point_to_consider, length=years_back*model_struct@freq+1, by=-1))
            allTimePoints = allTimePoints[allTimePoints>0]
            
            states = survts@state[,1]
            observed = observed(survts)
            if(model_struct@offset) {
              offset_denom = surveillance::population(survts)
            }
            else {
              offset_denom = rep(1, length(observed))
            }
            modelData = data.frame(response=observed[allTimePoints],
                                   wtime=0:(length(allTimePoints)-1),
                                   true_state=states[allTimePoints],
                                   rtime=allTimePoints,
                                   population=offset_denom[allTimePoints])
            
            if(model_struct@S>0) {
              for(i in 1:model_struct@S) {
                sin_name = paste("sin", i, sep="")
                cos_name = paste("cos", i, sep="")
                modelData[,sin_name] = 0 
                modelData[,cos_name] = 0 
                for(j in 1:i) {
                  if(j == model_struct@S) {
                    modelData[,sin_name] = modelData[,sin_name] + sin(2*pi*j*modelData$wtime/model_struct@freq)
                    modelData[,cos_name] = modelData[,cos_name] + cos(2*pi*j*modelData$wtime/model_struct@freq)
                  }
                }
              }
            }
            
            
            modelData
          })

setMethod("extractModelData", signature = c("sts", 
                                            "ExtData", 
                                            "numeric", "numeric"),
          function(survts, model_struct, time_point_to_consider, years_back) {
            allTimePoints = rev(seq(time_point_to_consider, length=round(years_back*model_struct@freq+1), by=-1))
            allTimePoints = allTimePoints[allTimePoints>0]
            
            states = survts@state[,1]
            observed = observed(survts)
            
            if(model_struct@offset) {
              offset_denom = surveillance::population(survts)
            }
            else {
              offset_denom = rep(1, length(observed))
            }
            
            modelData = data.frame(response=observed[allTimePoints],
                                   wtime=0:(length(allTimePoints)-1),
                                   true_state=states[allTimePoints],
                                   rtime=allTimePoints,
                                   population=offset_denom[allTimePoints])
            extdata = model_struct@extdata[allTimePoints,,drop=F]
            modelData = cbind(modelData, extdata)
            
            modelData
          })


setGeneric("addDistrData", function(distribution, modelData) standardGeneric("addDistrData"))

setMethod("addDistrData", signature = c("Poisson", "data.frame"),
          function(distribution, modelData) {
            modelData
          })
setMethod("addDistrData", signature = c("ZIPoisson", "data.frame"),
          function(distribution, modelData) {
            if(distribution@shared_pi) {
              modelData$shared_pi = "all"
            }
            else {
              modelData$shared_pi = modelData$id
            }
            modelData
          })
setMethod("addDistrData", signature = c("NegBinom", "data.frame"),
          function(distribution, modelData) {
            if(distribution@shared_dispersion) {
              modelData$shared_dispersion = "all"
            }
            else {
              modelData$shared_dispersion = modelData$id
            }
            modelData
          })
setMethod("addDistrData", signature = c("ZINegBinom", "data.frame"),
          function(distribution, modelData) {
            if(distribution@shared_pi) {
              modelData$shared_pi = "all"
            }
            else {
              modelData$shared_pi = modelData$id
            }
            if(distribution@shared_dispersion) {
              modelData$shared_dispersion = "all"
            }
            else {
              modelData$shared_dispersion = modelData$id
            }
            modelData
          })


setGeneric("prepareData", function(survts, hmm, time_point_to_consider,
                                   id, years_back=as.numeric(5), 
                                   past_weeks_not_included_training=as.numeric(0),
                                   past_weeks_not_included_state=as.numeric(26),
                                   past_weeks_not_included_init=as.numeric(26)) standardGeneric("prepareData"))

setMethod("prepareData", signature = c("sts", "DODmodel", "ANY",
                                       "ANY", "ANY", "ANY",
                                       "ANY", "ANY"
),
function(survts, hmm, time_point_to_consider,
         id, years_back, 
         past_weeks_not_included_training,
         past_weeks_not_included_state,
         past_weeks_not_included_init) {
  
  modelData = extractModelData(survts, hmm@emission@dod_formula, 
                               time_point_to_consider, years_back)
  #modelData$denom = log(survts@populationFrac[modelData$rtime])
  modelData$id = id
  modelData = addDistrData(hmm@emission@distribution, modelData)
  
  modelData$init = TRUE
  if(past_weeks_not_included_init>0) {
    start_ind = nrow(modelData)-past_weeks_not_included_init-1
    modelData$init[start_ind:nrow(modelData)] = FALSE
  }
  
  modelData$training = TRUE
  if(past_weeks_not_included_training>0) {
    start_ind = nrow(modelData)-past_weeks_not_included_training
    modelData$training[start_ind:nrow(modelData)] = FALSE
  }
  
  modelData$state_training = TRUE
  if(past_weeks_not_included_state>0) {
    start_ind = nrow(modelData)-past_weeks_not_included_state-1
    modelData$state_training[start_ind:nrow(modelData)] = FALSE
  }
  
  modelData$curr_week = FALSE
  modelData$curr_week[nrow(modelData)] = TRUE
  
  modelData
})

setMethod("prepareData", signature = c("list", "DODmodel", "ANY",
                                       "ANY", "ANY", "ANY",
                                       "ANY", "ANY"
),
function(survts, hmm, time_point_to_consider,
         id, years_back, 
         past_weeks_not_included_training,
         past_weeks_not_included_state,
         past_weeks_not_included_init) {
  
  if(length(names(survts)) == 0) {
    names(survts) = paste0("id", 1:length(survts))
  }
  
  modelData = list()
  for(n in names(survts)) {
    modelData[[n]] = prepareData(survts[[n]], hmm, time_point_to_consider,
                                 id=n, years_back, 
                                 past_weeks_not_included_training,
                                 past_weeks_not_included_init)
  }
  
  do.call("rbind", modelData)
})

