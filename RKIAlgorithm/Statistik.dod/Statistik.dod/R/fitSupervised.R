

fitSupervised = function(hmm, modelData) {
  
  if(length(unique(modelData$id)) == 1 & 
     !hmm@emission@dod_formula@shared_params) {
    hmm@emission@dod_formula@shared_params = TRUE
    formula_bckg = createFormula(hmm@emission@distribution, 
                                 hmm@emission@dod_formula)
    formula = paste0(formula_bckg, " + state")
    hmm@emission@dod_formula@formula = formula
    hmm@emission@dod_formula@formula_bckg = formula_bckg
  }
  
  modelData$known_state = modelData$true_state
  modelData$state = modelData$true_state

  dataGLM1 = modelData
  dataGLM1$state = 0
  dataGLM2 = modelData
  dataGLM2$state = 1
  dataGLM_expanded = rbind(dataGLM1, dataGLM2)
  
  emission_params = data.frame(mu=rep(NA, nrow(dataGLM_expanded)), 
                               dispersion=rep(NA, nrow(dataGLM_expanded)),
                               pi=rep(NA, nrow(dataGLM_expanded)))
  
  emission_params = emission_params[getParamNames(hmm@emission@distribution)]
  dataGLM_expanded = cbind(dataGLM_expanded, emission_params)
  
  
  omega = as.numeric(dataGLM_expanded$state == dataGLM_expanded$known_state)
  omega[!dataGLM_expanded$state_training] = 0
  
  model_updated = updateEmission(hmm@emission, dataGLM_expanded, omega)
  hmm@emission = model_updated$emission
  dataGLM_expanded = model_updated$model
  
  modelData_fit = modelData[modelData$state_training,] 
  modelData_fit_id = split(modelData_fit, modelData_fit$id)
  transMat = lapply(modelData_fit_id, function(x) {
    table(factor(x$state[1:(nrow(x)-1)], levels=c(0,1)), 
          factor(x$state[2:nrow(x)], levels=c(0,1)))
  })
  transMat = Reduce("+", transMat)
  transMat = t(apply(transMat, 1, function(x) x/sum(x)))
  
  initProb = lapply(modelData_fit_id, function(x) {
    table(factor(x$state[1], levels=c(0,1)))
  })
  initProb = Reduce("+", initProb)
  initProb = initProb/sum(initProb)

  hmm@transitions = as.matrix(transMat)
  hmm@initial_prob = as.vector(initProb)
  
  emission_prob = calcEmissionProb(hmm@emission@distribution, dataGLM_expanded)
  dataGLM_expanded$known_state[!dataGLM_expanded$state_training]=NA
  hmm_expectation = forwardBackward(hmm, dataGLM_expanded, emission_prob)
  
  
  
  list(hmm=hmm, hmm_expectation=hmm_expectation, model=dataGLM_expanded, 
       loglik=hmm_expectation$LogLik, niter=1)
}  

