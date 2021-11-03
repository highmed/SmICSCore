getAlphaBeta = function(y, transMat, initProb, emissionProb, states=rep(NA, length(y))) {
  
  known_outbreaks = which(states==1)
  if(length(known_outbreaks) > 0) {
    emissionProb[known_outbreaks,1] = 0
  }
  known_endemic = which(states==0)
  if(length(known_endemic) > 0) {
    emissionProb[known_endemic,2] = 0
  }
  
  resc = rep(0, length(y))
  alpha = matrix(0, nrow=length(y), ncol=ncol(transMat))
  beta = matrix(0, nrow=length(y), ncol=ncol(transMat))
  
  # Compute alpha_1(i) and rescale with c_1
  for(i in 1:ncol(transMat)) {
    alpha[1,i] = initProb[i]*emissionProb[1,i]
    resc[1] = resc[1] + alpha[1,i]
  }
  resc[1] = 1/resc[1]
  
  for(i in 1:ncol(transMat)) {
    alpha[1,i] = resc[1]*alpha[1,i]
  }
  
  
  # Compute all other alpha_t(i)
  for(t in 2:length(y)) {
    resc[t] = 0
    for(i in 1:nrow(transMat)) {
      alpha[t,i] = 0
      for(j in 1:ncol(transMat)) {
        alpha[t,i] = alpha[t,i] + alpha[t-1,j]*transMat[j,i]
      }
      alpha[t,i] = alpha[t,i]*emissionProb[t,i]
      resc[t] = resc[t] + alpha[t,i]
      
    }
    # Rescale alpha[t][i]
    resc[t] = 1/resc[t]
    
    for(i in 1:nrow(transMat)) {
      alpha[t,i] = resc[t]*alpha[t,i]
    }
  }
  
  
  # beta_T = 1 is rescaled
  for(i in 1:ncol(transMat)) {
    beta[length(y), i] = 1
  }
  
  # Compute all other beta_t(i)
  for(t in (length(y)-1):1) {
    for(i in 1:nrow(transMat)) {
      beta[t,i] = 0
      for(j in 1:ncol(transMat)) {
        beta[t,i] = beta[t,i]+transMat[i,j]*emissionProb[t+1,j]*beta[t+1,j]
      }
      beta[t,i] = resc[t]*beta[t,i]
    }
  }
  
  return(list(resc=resc, alpha=alpha, beta=beta))
}


getGammaXsi = function(hmm, modelData, emissionProb) {
  
  
  y = modelData$response[1:(nrow(modelData)/2)]
  transMat = hmm@transitions
  initProb = hmm@initial_prob
  states = modelData$known_state[1:(nrow(modelData)/2)]
  
  known_outbreaks = which(states==1)
  if(length(known_outbreaks) > 0) {
    emissionProb[known_outbreaks,1] = 0
  }
  
  known_endemic = which(states==0)
  if(length(known_endemic) > 0) {
    emissionProb[known_endemic,2] = 0
  }
  
  alpha_beta = getAlphaBeta(y, transMat, initProb, emissionProb, states)
  
  alpha = alpha_beta$alpha
  beta = alpha_beta$beta
  resc = alpha_beta$resc
  
  gamma = matrix(0, nrow=nrow(alpha), ncol=ncol(alpha))
  xsi = lapply(1:length(y), function(x) matrix(0, nrow=ncol(alpha), ncol=ncol(alpha)))
  for(t in 1:nrow(alpha)) {
    
    # Calculate gamma
    denom = 0
    for(i in 1:ncol(alpha)) {
      gamma[t,i] = alpha[t,i] * beta[t,i]
      denom = denom + gamma[t,i]
    }
    for(i in 1:ncol(alpha)) {
      gamma[t,i] = gamma[t,i]/denom
    }
    # Calculate xsi
    if(t<nrow(alpha)) {
      for(i in 1:ncol(alpha)) {
        denom = 1/resc[t] * beta[t,i];
        for(j in 1:ncol(alpha)) {
          xsi[[t]][i,j] = (gamma[t,i]*transMat[i,j]*emissionProb[t+1,j] * beta[t+1,j])  / denom
        }
      }
    }
  }
  return(list(LogLik=-sum(log(resc)), gamma=gamma, xsi=xsi))
}


forwardBackward = function(hmm, modelData, emissionProb) {
  modelDataById = tapply(1:nrow(modelData), INDEX=modelData$id, identity)
  emissionProb = as.vector(emissionProb)
  modelDataById = modelDataById[order(sapply(modelDataById,min))]
  hmm_expectation = list()
  for(i in 1:length(modelDataById)) {
    hmm_expectation[[i]] = getGammaXsi(hmm, 
                                       modelData[sort(modelDataById[[i]]),], 
                                       matrix(emissionProb[modelDataById[[i]]], ncol=2))
  }

  return(list(LogLik=sum(sapply(hmm_expectation, function(x) x$LogLik)),
              gamma=do.call("rbind", lapply(hmm_expectation, function(x) x$gamma)),
              xsi=unlist(lapply(hmm_expectation, function(x) x$xsi), recursive=F)))
  
}
