
fitUnsupervised = function(hmm, modelData, transMat_init, learning_type="unsupervised", maxIter, verbose, years_back) {

  if(length(unique(modelData$id)) == 1 & 
     !hmm@emission@dod_formula@shared_params) {
    hmm@emission@dod_formula@shared_params = TRUE
    formula_bckg = createFormula(hmm@emission@distribution, 
                                 hmm@emission@dod_formula)
    formula = paste0(formula_bckg, " + state")
    hmm@emission@dod_formula@formula = formula
    hmm@emission@dod_formula@formula_bckg = formula_bckg
  }
  
  model_init = initGLM(hmm, modelData, learning=learning_type, setBckgState = hmm@setBckgState)#, learning="semi-supervised")
  model = model_init$model
  hmm@emission = model_init$emission
  
  nid = length(unique(modelData$id))
  base_weight =  matrix(min(c(nrow(modelData), years_back*hmm@emission@dod_formula@freq*nid)), nrow=2, ncol=2)
  # Calculate stationary state distribution
  n = ncol(transMat_init)
  A = t(transMat_init - diag(n))
  A = rbind(A, rep(1, n))
  b = c(rep(0, n), 1)
  stationary_state = qr.solve(A, b)
  # Calculate prior weights
  prior_weights = transMat_init*base_weight*stationary_state
  
    
#    base_weight =  matrix(nrow(modelData), nrow=2, ncol=2)
#    prior_weights = transMat_init*base_weight*t(mpow(transMat_init, 50))
  
#    nid = length(unique(modelData$id))
#    base_weight =  matrix(min(c(nrow(modelData), years_back*hmm@emission@dod_formula@freq*nid)), nrow=2, ncol=2)
#    prior_weights = transMat_init*base_weight*t(mpow(transMat_init, 50))
#    print(prior_weights)
#    print(t(mpow(transMat_init, 50)))

  
  hmm@prior_weights = prior_weights
  initProb = hmm@initial_prob
  
  emission_prob = calcEmissionProb(hmm@emission@distribution, model) 
  
  hmm_expectation = forwardBackward(hmm, model, emission_prob)
  old_loglik = -Inf

  if(hmm@transitions_prior) {
    log_dir1 = log_dirichlet(hmm@transitions[1,], hmm@prior_weights[1,])
    log_dir2 = log_dirichlet(hmm@transitions[2,], hmm@prior_weights[2,])
    hmm@loglik_transitions = log_dir1 + log_dir2
  }
  
  new_loglik = hmm_expectation$LogLik + hmm@loglik_transitions
  curr_diff = Inf
  
  if(verbose) {
    cat(" Iter 0: LogLik=", new_loglik, " (diff=", curr_diff, ")\n", sep="")
  }
  
  niter = 1
  
  # EM iterations
  while(curr_diff > 1e-6 & niter <= maxIter) {
    
    old_loglik = new_loglik
    
    omega = as.vector(hmm_expectation$gamma)
    model_updated = updateEmission(hmm@emission, model, omega)
    hmm@emission = model_updated$emission
    model = model_updated$model
    hmm = updateTransInitProb(hmm, hmm_expectation$gamma, hmm_expectation$xsi, model) 
    
    emission_prob = calcEmissionProb(hmm@emission@distribution, model)
    hmm_expectation = forwardBackward(hmm, model, emission_prob)
    
    new_loglik = hmm_expectation$LogLik + hmm@loglik_transitions
    
    curr_diff = (new_loglik-old_loglik)/abs(old_loglik)
    if(verbose) {
      cat(" Iter ", niter, ": LogLik=", new_loglik, " (diff=", curr_diff, ")\n", sep="")
    }
    niter = niter+1
    
  }
  
  if(niter > maxIter) {
    warning("Maximum number of iterations exceeded in EM.")
  }
  
  if(curr_diff < 0) {
    stop("Decrease in Log-likelihood during EM.\n")
  }
  
  list(hmm=hmm, hmm_expectation=hmm_expectation, model=model, 
       loglik=new_loglik, niter=niter)
}