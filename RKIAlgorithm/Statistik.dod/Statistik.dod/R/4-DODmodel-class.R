
#' This class is a generic container for the model used for outbreak detection
#' 
#' @slot emission Emission of hidden Markov model. Includes a DODfamily and a DODformula object.
#' @slot transitions Transition proabilities of hidden Markov model.
#' @slot transitions_prior Indicates whether a prior for transition probabilities should be used.
#' @slot prior_weights Dirichlet prior weights for transition probabilities. This cannot be changed by the user.
#' @slot loglik_transitions Prior log likelihood of transition probabilities. Only relevant for model fitting.
#' @slot initial_prob Initial state probabilities of the hidden Markov model.
#' @slot setBckgState Indicates whether a background state should be computed for model fitting. Background states are set to 0s and time points where the Anscombe residuals of the initial model are < 1.
#' 
#' @exportClass DODmodel
setClass("DODmodel",
         slots = c(
           emission="Emission",
           transitions="matrix",
           transitions_prior="logical",
           prior_weights="matrix",
           loglik_transitions="numeric",
           initial_prob="numeric",
           setBckgState="logical"
         )
)

#' Prints description of FarringtonNoufaily object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("DODmodel"), function(object) {
  print(object@emission@distribution)
  print(object@emission@dod_formula)
  transMat=paste(round(c(object@transitions[1,], object@transitions[2,]),2), collapse=", ")
  cat("transitions=c(", transMat, ")\n",  
      "initial_prob=c(", paste(object@initial_prob, collapse=", "), ")\n",  
      "transitions_prior=", object@transitions_prior, "\n",  
      "setBckgState=", object@setBckgState, "\n", sep="")
 
})



#' @title Create a model for outbreak detection
#' 
#' @param family A DODfamily object specifying the family of probability distributions.
#' @param formula A DODformula object specifying the formula of the model.
#' @param transMat Inital transition probabilities.
#' @param transMat_prior Indicates whether a prior for transition probabilities should be used.
#' @param setBckgState Indicates whether a background state should be computed for model fitting. Background states are set to 0s and time points where the Anscombe residuals of the initial model are < 1.
#' 
#' @usage DODmodel(family, formula, transMat = NULL, initProb = NULL, transMat_prior = TRUE, setBckgState = TRUE)
#' 
#' @seealso \code{\linkS4class{DODmodel}}, \code{\linkS4class{DODfamily}}, \code{\linkS4class{DODformula}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
DODmodel = function(family, formula, 
                    model = "GLM",
                    transMat = NULL, 
                    initProb = NULL,
                    transMat_prior = TRUE,
                    setBckgState = TRUE) {
  
  emission = Emission(family, formula, model)
  
  if(is.null(transMat)) {
    transMat = matrix(c(0.95,0.4,0.05,0.6), ncol=2)
  }
  if(is.null(initProb)) {
    initProb = c(0.5, 0.5)
  }
  
  new("DODmodel", emission=emission,
      transitions=transMat,
      transitions_prior=transMat_prior,
      prior_weights=matrix(NA, nrow=2, ncol=2),
      loglik_transitions=as.numeric(0),
      initial_prob=initProb,
      setBckgState=setBckgState)
}

updateTransInitProb = function(dod_model, gamma, xsi, modelData) {
  
  log_dir1 = 0
  log_dir2 = 0
  
  transMat = Reduce("+", xsi) 
  gammaSum_ind = which(modelData$wtime!=max(modelData$wtime) & modelData$state==0)
  #print(gammaSum_ind)
  gammaSum = apply(gamma[gammaSum_ind,],2,sum) 
  if(dod_model@transitions_prior) { # Update with prior weights
    for(i in 1:nrow(transMat)) {
      transMat[i,] = (dod_model@prior_weights[i,] - 1 + transMat[i,])/(sum(dod_model@prior_weights[i,]) - 2 + gammaSum[i])
    }
    log_dir1 = log_dirichlet(transMat[1,], dod_model@prior_weights[1,])
    log_dir2 = log_dirichlet(transMat[2,], dod_model@prior_weights[2,])
  }
  else { # Update without prior
    for(i in 1:nrow(transMat)) {
      transMat[i,] = transMat[i,]/gammaSum[i]
    }
  }
  
  initProb_ind = which(modelData$wtime==0 & modelData$state==0)
  initProb = gamma[initProb_ind,]
  if(length(initProb_ind)>1) {
    initProb = apply(initProb,2,sum)
  }
  initProb = initProb/sum(initProb)
  
  dod_model@transitions=transMat
  dod_model@initial_prob=initProb
  dod_model@loglik_transitions=log_dir1+log_dir2
  
  dod_model
}
