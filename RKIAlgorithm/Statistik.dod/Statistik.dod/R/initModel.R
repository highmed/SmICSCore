


# Initialization procedure for semi- and unsupervised learning
initGLM = function(hmm, 
                   modelData, 
                   learning="unsupervised",
                   weightsThreshold=2.58, 
                   weightsThresholdBckg=1, 
                   setBckgState=TRUE, 
                   limitCaseWeeks=c(-Inf,1)) {
 
  #print(modelData$init)
  #print(table(modelData$init))
  suppressWarnings(curr_model <- glm(hmm@emission@dod_formula@formula_bckg,data=modelData[modelData$init,], 
                                     family = quasipoisson(link="log")))

#  if(weightsThreshold<Inf) {
#    curr_anscombe_res_temp <- surveillance::anscombe.residuals(curr_model, max(summary(curr_model)$dispersion,1))
#    omega <- surveillance::algo.farrington.assign.weights(curr_anscombe_res_temp,weightsThreshold)
#    curr_formula = as.formula(hmm@emission@dod_formula@formula_bckg)
#    suppressWarnings(curr_model <- glm(curr_formula,data=modelData[modelData$init,], 
#                                       family = quasipoisson(link="log"), weights=omega))
#  }
  
  phi <- max(summary(curr_model)$dispersion,1)
  K = min(c(10,phi+1))

  curr_anscombe_res_temp <- surveillance::anscombe.residuals(curr_model, max(summary(curr_model)$dispersion,1))
  curr_anscombe_res <- rep(Inf, nrow(modelData))
  curr_anscombe_res[which(modelData$init)] <- curr_anscombe_res_temp # & bckg_state
  
  # Use anscombe residuals to set bckg states
  modelData$bckg_state = modelData$true_state
  
  modelData$bckg_state = NA
  if(setBckgState) {
    selInd = which(!modelData$curr_week)
    setZero = which(curr_anscombe_res[selInd]<weightsThresholdBckg | modelData$response[selInd]==0)
    modelData$bckg_state[setZero] = 0

  }
  
  # Set states which should not be used for training to NA
  modelData$true_state[!modelData$state_training] = NA
  modelData$bckg_state[!modelData$state_training] = NA
  
  if(learning=="unsupervised" | (learning == "semisupervised" & all(is.na(modelData$true_state)))) {
    modelData$known_state = modelData$bckg_state
  }
  else {
    modelData$known_state = modelData$true_state
  }
  #write.table(modelData, file="data_dod.csv", sep=";", quote=FALSE, row.names=FALSE)
  
  dataGLM1 = modelData
  dataGLM2 = modelData
  dataGLM1$state=0
  dataGLM2$state=1
  dataGLM_expanded = rbind(dataGLM1, dataGLM2)
  
  expected_mu_bckg <- predict(curr_model, newdata=modelData,type="response")
  expected_mu = c(expected_mu_bckg, expected_mu_bckg*K)
  start_pi = sum(as.numeric(dataGLM_expanded$response==0))/(nrow(dataGLM_expanded))
  emission_params = data.frame(mu=expected_mu, dispersion=expected_mu/(phi-1),
                               pi=start_pi)
  take_cols = names(dataGLM_expanded)
  dataGLM_expanded = cbind(dataGLM_expanded, emission_params)
  
  # Call generic function for Negative Binomial for initialization
  emissionProb = calcEmissionProb(new("NegBinom"), dataGLM_expanded) 
  
  dataGLM_expanded = dataGLM_expanded[,c(take_cols, getParamNames(hmm@emission@distribution))]
  
  both_zero = which(apply(emissionProb,1,sum)<.Machine$double.eps)
  if(length(both_zero)>0) {
    for(k in both_zero) {
      assign_state = ifelse(modelData$response[k]>expected_mu[k], 1, 0)
      emissionProb[k,] = c(1-assign_state, assign_state)
    }
  }
  
  
  gamma_xsi = getGammaXsi(hmm, dataGLM_expanded, emissionProb)
  #write.table(gamma_xsi$gamma, file="posterior_dod.csv", sep=";", quote=FALSE, row.names=FALSE)
  
  omega = as.vector(gamma_xsi$gamma)
  model_updated = updateEmission(hmm@emission, dataGLM_expanded, omega)
  model_updated
}  

