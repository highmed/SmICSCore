

#' Disease outbreak detection - package main function
#'
#' @title Detect disease outbreaks from single or multiple time series of case counts
#' 
#' @param surv_ts Either a surveillance time series (sts) object created using the surveillance package or a list of sts objects.
#' @param dod_model An object of class \code{\linkS4class{DODmodel}} which specifies the model parameters and structure
#' @param timepoints Specifying whether confidence intervals should be computed, default: TRUE.
#' @param learning_type Indicates the type of learning, one of c("unsupervised", "semisupervised", "supervised")
#' @param maxIter Maximal number of iteration for EM-algorithm.
#' @param verbose Logical indicating wehther progress should be printed.
#' 
#' @usage dod(surv_ts, dod_model, timepoints, learning_type="unsupervised", maxIter=100, verbose=FALSE)
#' 
#' @seealso \code{\linkS4class{DODmodel}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
dod = function(surv_ts, dod_model, timepoints, learning_type="unsupervised", maxIter=100, 
               verbose=FALSE, return_full_model=FALSE, past_weeks_not_incuded=26, years_back=5,
               return_coef=FALSE, return_past_posterior=0, return_transitions=FALSE, 
               return_init_prob=FALSE) { 
  
  transMat_init = dod_model@transitions
  initProb_init = dod_model@initial_prob
  
  all_errors = rep("", length(timepoints))
  names(all_errors) = timepoints
  
  result = list()
  for(k in timepoints) {
    
    if(verbose) {
      cat("Fitting model at position ", k, "\n", sep="")
    }
    
    dod_model@transitions = transMat_init
    dod_model@initial_prob = initProb_init
    
    dod_model_fit = NULL
    
    curr_error = tryCatch({
      modelData = prepareData(surv_ts, dod_model, k, id="ts1", years_back=years_back,
                              past_weeks_not_included_state=past_weeks_not_incuded,
                              past_weeks_not_included_init=past_weeks_not_incuded)
      curr_k = k
      if(return_full_model) {
        curr_k = modelData$rtime
      }
      result[[as.character(k)]] = createEmptyResult(dod_model, curr_k, modelData$id[which(modelData$rtime == k)])
      
      if(learning_type %in% c("unsupervised", "semisupervised")) {
        dod_model_fit = fitUnsupervised(dod_model, modelData, transMat_init, learning_type=learning_type, maxIter, verbose, years_back=years_back)
      }
      else if(learning_type == "supervised") {
        dod_model_fit = fitSupervised(dod_model, modelData)
      }
      else {
        stop("learning_type must be one of: unsupervised, semisupervised or supervised.\n")
      }
      if(dod_model_fit$hmm@emission@dod_formula@failed_optim/dod_model_fit$niter > 1/3) {
        nfails = dod_model_fit$hmm@emission@dod_formula@failed_optim
        stop("Optimization of ", dod_model_fit$hmm@emission@distribution@name, 
             " failed in ", nfails, " out of ", dod_model_fit$niter, " EM-iterations.\n", sep="")
      }
      
      NULL
      
    }, error = function(e) {
      warning("Error fitting model at position ", k, ": ", as.character(e), ".")
      paste("Error fitting model at position ", k, ": ", as.character(e), ".", sep="")
    })
    
    if(!is.null(curr_error)) {
      print(curr_error)
      all_errors[as.character(k)] = curr_error
    }
    else {
      # Prepare output
      curr_k = k
      if(return_full_model) {
        curr_k = modelData$rtime
      }
      result[[as.character(k)]] = createResult(curr_k, dod_model_fit$hmm, dod_model_fit$model, 
                                               dod_model_fit$hmm_expectation, dod_model_fit$loglik,
                                               return_coef)
      if(return_past_posterior>0) {
        nRow = nrow(dod_model_fit$hmm_expectation$gamma)-1
        start = max(c(1,(nRow-return_past_posterior+1)))
        past_posterior = dod_model_fit$hmm_expectation$gamma[start:nRow,2]
        past_posterior = as.data.frame(matrix(past_posterior, ncol=return_past_posterior))
        names(past_posterior) = paste0("posterior_", result[[as.character(k)]]$timepoint-(return_past_posterior:1))
        result[[as.character(k)]] = cbind(result[[as.character(k)]], past_posterior)
      }
      
      if(return_transitions) {
        transitions = as.vector(t(dod_model_fit$hmm@transitions))
        names(transitions) = c("A_00", "A_01", "A_10", "A_11")
        result[[as.character(k)]] = cbind(result[[as.character(k)]], as.data.frame(t(transitions)))
      }
      
      if(return_init_prob) {
        init_prob = as.vector(t(dod_model_fit$hmm@initial_prob))
        names(init_prob) = c("pi_0", "pi_1")
        result[[as.character(k)]] = cbind(result[[as.character(k)]], as.data.frame(t(init_prob)))
      }
    }
    
  }
  result = do.call("rbind", result)
  
  if(any(all_errors!="")) {
    result$error = ""
    timepoint2index = tapply(1:nrow(result), INDEX=result$timepoint, identity)
    for(i in names(timepoint2index)) {
      result$error[timepoint2index[[i]]] = all_errors[i]
    }
  }
  
  result
}

createEmptyResult = function(dod_model, k, id) {
  result = data.frame(posterior=NA, 
             pval=NA, 
             timepoint=k,
             observed=NA,
             mu0=NA,
             mu1=NA,
             id=id,
             offset=NA)
  
  params = getParamNames(dod_model@emission@distribution)
  params = params[params != "mu"]
  if(length(params)>0) {
    start_index = ncol(result)
    na = rep(NA, nrow(result)*length(params))
    result = cbind(result, matrix(na, ncol=length(params)))
    names(result)[(start_index+1):ncol(result)] = params
  }
  result$bic = NA
  result$aic = NA
  result
}


createResult = function(k, dod_model, model, dod_model_expectation, loglik, return_coef) {
  index_0 = which(model$rtime %in% k & model$state == 0)
  index_1 = which(model$rtime %in% k & model$state == 1)
  result = data.frame(posterior=dod_model_expectation$gamma[index_0,2], 
                                         pval=NA, 
                                         timepoint=k,
                                         observed=model$response[index_0],
                                         mu0=model$mu[index_0],
                                         mu1=model$mu[index_1],
                                         id=model$id[index_0],
                                         offset=model$population[index_1])
  params = getParamNames(dod_model@emission@distribution)
  params = params[params != "mu"]
  
  if(length(params) > 0) {
    start_index = ncol(result)
    result = cbind(result, model[index_0,params])
    names(result)[(start_index+1):ncol(result)] = params
  }
  result$pval = calculatePvalue(dod_model@emission@distribution, 
                                                   result)
  npar = length(dod_model@emission@dod_formula@coefficients) + 
    getSharedParN(dod_model@emission@distribution)*length(unique(result$id)) + 6
  result$bic = log(nrow(dod_model_expectation$gamma))*npar - 2*loglik
  result$aic = -2*npar - 2*loglik
  rownames(result) = NULL
  
   if(return_coef) {
    
    coeff = dod_model@emission@dod_formula@coefficients
    model_0 = coeff[setdiff(1:length(coeff), grep("id", names(coeff)))]
    
    models = list()
    for(curr_id in result$id) {
      curr_coeff = coeff[grep(curr_id, names(coeff))]
      curr_coeff
      names(curr_coeff) = gsub(paste0("id", curr_id, ":*"), "", names(curr_coeff))
      names(curr_coeff)[names(curr_coeff)==""] = "(Intercept)"
      
      curr_model = model_0
      curr_model[names(curr_coeff)] = curr_model[names(curr_coeff)] + curr_coeff
      models[[curr_id]] = curr_model
    }
    models = do.call("rbind", models)
    
    result = cbind(result,as.data.frame(models))
    result$formula = gsub("id", "1", gsub("\\*id", "", dod_model@emission@dod_formula@formula))
  }
  
  result
}


