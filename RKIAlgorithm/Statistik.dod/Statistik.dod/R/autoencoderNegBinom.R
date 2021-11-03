
setClass("Autoencoder",
         slots = c(
           X="matrix",
           K="matrix",
           offset="matrix",
           W1="matrix",
           W2 = "matrix",
           b="numeric",
           q="numeric"
         )
)

setClass("AutoencoderNegBinom",
         contains = "Autoencoder",
         slots = c(
           theta="numeric"
         )
)


setGeneric("encode", function(autoencoder) standardGeneric("encode"))

setMethod("encode", signature = c("AutoencoderNegBinom"),
          function(autoencoder) {
            autoencoder@X %*% autoencoder@W1
          })


setGeneric("decode", function(autoencoder) standardGeneric("decode"))

setMethod("decode", signature = c("AutoencoderNegBinom"),
          function(autoencoder) {
            X = autoencoder@X
            K = autoencoder@K
            W1 = autoencoder@W1
            W2 = autoencoder@W2
            b = autoencoder@b
            offset_mat = log(autoencoder@offset)
            
            # Create matrices for gradient calculation
            B = matrix(rep(b,nrow(K)), nrow=nrow(K), byrow=T)
            mu = exp(X %*% W1 %*% t(W2) + B + offset_mat)
            mu
          })


setGeneric("getPar", function(autoencoder) standardGeneric("getPar"))

setMethod("getPar", signature = c("AutoencoderNegBinom"),
          function(autoencoder) {
            c(autoencoder@W1, autoencoder@W2,
              autoencoder@b,autoencoder@theta)
            
          })

setGeneric("setPar", function(par, autoencoder) standardGeneric("setPar"))

setMethod("setPar", signature = c("numeric", "AutoencoderNegBinom"),
          function(par, autoencoder) {
            
            q = autoencoder@q
            nsegm = length(par)/(q*2+2)
            n_W1 = length(autoencoder@W1)
            autoencoder@W1 = matrix(par[1:n_W1], ncol=q) 
            n_W2 = length(autoencoder@W2)
            autoencoder@W2 = matrix(par[(n_W1+1):(n_W1+n_W2)], ncol=q) 
            n_b = length(autoencoder@b)
            autoencoder@b = par[(n_W1+n_W2+1):(n_W1+n_W2+n_b)]
            autoencoder@theta = exp(par[(n_W1+n_W2+n_b+1):length(par)])
            autoencoder
            
          })

AutoencoderNegBinom = function(K, offset_mat, q, pseudocount) {
  
  X = log(K+pseudocount)
  
  pcs <- prcomp(X, center = TRUE, scale. = FALSE)
  W1 = matrix(pcs$rotation[,1:q], ncol=q)
  W2 = matrix(rep(0,ncol(K)), ncol=q)
  b = rep(0, ncol(K))#pcs$center
  theta = rep(1, ncol(K))
  
  hidden_act = X %*% W1
  for(i in 1:ncol(K)) {
    curr_dat = data.frame(response=K[,i], h1=hidden_act[,1], norm_fac=offset_mat[,i])
    
    err = tryCatch({
      suppressWarnings(curr_model <- glm.nb("response ~ 1 + h1 + offset(log(norm_fac))", data=curr_dat))
      if(! any(is.na(curr_model$coefficients))) {
        W2[i,1] = curr_model$coefficients["h1"]
        b[i] = curr_model$coefficients["(Intercept)"]
        theta[i] = log(curr_model$theta)
      }
    }, error = function(e) {
    #  print(e)
     TRUE
      
    })
    
    if(err) {
      suppressWarnings(curr_model <- glm("response ~ 1 + h1 + offset(log(norm_fac))", data=curr_dat, family=poisson))
      if(! any(is.na(curr_model$coefficients))) {
        W2[i,1] = curr_model$coefficients["h1"]
        b[i] = curr_model$coefficients["(Intercept)"]
        theta[i] = log(1)
      }
    }
    
  }

  new("AutoencoderNegBinom",
      X=X, K=K, offset=offset_mat,
      W1=W1, W2=W2,
      b=b, q=q, theta=theta)
  
}

setGeneric("gradientAutoencoder", function(par, autoencoder) standardGeneric("gradientAutoencoder"))

setMethod("gradientAutoencoder", signature = c("numeric", "AutoencoderNegBinom"),
          function(par, autoencoder) {
            
            # Get and set current parameters
            K = autoencoder@K
            X = autoencoder@X
            q = autoencoder@q
            autoencoder = setPar(par, autoencoder)
            W1 = autoencoder@W1
            W2 = autoencoder@W2
            b = autoencoder@b
            theta = autoencoder@theta
            offset_mat = log(autoencoder@offset)
            
            # Create matrices for gradient calculation
            B = matrix(rep(b,nrow(K)), nrow=nrow(K), byrow=T)
            mu = exp(X %*% W1 %*% t(W2) + B + offset_mat)
            mu = mu+1e-12
            mu[mu==Inf] = 1e6
            Theta = matrix(rep(theta,nrow(K)), nrow=nrow(K), byrow=T)
            
            # Compute terms for gradient computation
            U = ((K + Theta)*mu)/(Theta+mu)
            phi = X %*% W1
            phi1 = matrix(1, nrow=nrow(X), ncol=ncol(W1))
            
            # Gradient of weights
            gradW1 = t(X) %*% ((K %*% W2) * phi1) - t(X) %*% ((U %*% W2) * phi1)
            gradW2 = t(K) %*% phi - t(U) %*% phi
            gradW = c(gradW1, gradW2)
      
            # Gradient of bias
            gradb = apply(K-U, 2, sum)
            
            # Gradient of theta
            gradTheta <- apply(Theta * (digamma(K + Theta) - 
                                               digamma(Theta) + log(Theta) - log(mu + Theta) + 1 - 
                                               (K + Theta)/(mu + Theta)),2,sum)
            
            c(-gradW,-gradb,-gradTheta)
            
            
          })


setGeneric("negLogLikAutoencoder", function(par, autoencoder) standardGeneric("negLogLikAutoencoder"))

setMethod("negLogLikAutoencoder", signature = c("numeric", "AutoencoderNegBinom"),
          function(par, autoencoder) {
            
            # Get and set current parameters
            K = autoencoder@K
            X = autoencoder@X
            q = autoencoder@q
            autoencoder = setPar(par, autoencoder)
            W1 = autoencoder@W1
            W2 = autoencoder@W2
            b = autoencoder@b
            theta = autoencoder@theta
            offset_mat = log(autoencoder@offset)
            
            # Create matrices for gradient calculation
            B = matrix(rep(b,nrow(K)), nrow=nrow(K), byrow=T)
            mu = exp(X %*% W1 %*% t(W2) + B + offset_mat)
            mu = mu+1e-12
            mu[mu==Inf] = 1e6
            Theta = matrix(rep(theta,nrow(K)), nrow=nrow(K), byrow=T)
            
            -sum(dnbinom(as.vector(K), 
                         mu=as.vector(mu), 
                         size=as.vector(Theta), 
                         log=TRUE))

          })



#' @title Fit a linear count Autoencoder
#' 
#' @param K Input matrix of case counts. Columns indicate regions (e.g. counties), rows indicate time points.
#' @param offset_mat An offset to included in the model.
#' @param q Number of hidden units. Default is 1.
#' @param pseudocount Number of pseudocounts to be added to case counts.
#' @param maxIter Maximal number of iterations for model fitting.
#' @param verbose Indicates whether progress should be printed during model fitting.
#' 
#' @usage fitAutoencoder(K, offset_mat=matrix(1,nrow=nrow(K),ncol=ncol(K)), q=1, pseudocount=1, maxIter=1000, verbose=F) 
#'  
#' @examples
#' 
#' TODO
#'   
#' @export
fitAutoencoder = function(K, offset_mat=matrix(1,nrow=nrow(K), ncol=ncol(K)),
                          q=1, pseudocount=1, maxIter=1000, verbose=F) {

  autoencoder = tryCatch({
    
    autoencoder = AutoencoderNegBinom(K, offset_mat, q, pseudocount) 
    initpar = getPar(autoencoder)
    
    fit = NULL
    fit = tryCatch({
      optim(initpar, fn=negLogLikAutoencoder, gr=gradientAutoencoder, autoencoder=autoencoder, method="L-BFGS", 
                   control=list(trace=as.numeric(verbose), REPORT=1, maxit=maxIter))
    }, error = function(e) {
      print(e)
      optim(initpar, fn=negLogLikAutoencoder, gr=gradientAutoencoder, autoencoder=autoencoder, method="BFGS", 
                   control=list(trace=as.numeric(verbose), REPORT=1, maxit=maxIter))
    })
    
    # Set parameters
    autoencoder = setPar(fit$par, autoencoder)
    
  
  }, error = function(e) {
    print(e)
      NULL
  })
  
  autoencoder
  
}


#' @title Extract hidden representation of cases using a linear count Autoencoder
#' 
#' @param K Input matrix of case counts. Columns indicate regions (e.g. counties), rows indicate time points.
#' @param offset_mat An offset to included in the model.
#' @param myrange Range of time points for which hidden representation should be calculated.
#' @param time_back Number of previous time points which should be included in the model.
#' @param q Number of hidden units. Default is 1.
#' @param pseudocount Number of pseudocounts to be added to case counts.
#' @param maxIter Maximal number of iterations for model fitting.
#' @param verbose Indicates whether progress should be printed during model fitting.
#' 
#' @usage extractFeatures(K, offset_mat=matrix(1,nrow=nrow(K),ncol=ncol(K)), myrange=NULL, time_back=260, q=1, pseudocount=1, maxIter=1000, verbose=F) 
#'  
#' @examples
#' 
#' TODO
#'   
#' @export
extractFeatures = function(K, myrange=NULL, time_back=260,
                           q=1, pseudocount=1, shift_feature=52,  
                           offset_mat=matrix(1,nrow=nrow(K),ncol=ncol(K)),
                           maxIter=1000, verbose=F) {
  
  if(is.null(myrange)) {
    myrange=nrow(K)  
  }
  hidden_layers = list()
  
  for(i in 1:length(myrange)) {
    if(verbose) {
      cat("Fitting position ", myrange[i], ".\n", sep="")
    }
      
    hidden_layers[[i]] = matrix(NA, nrow=nrow(K), ncol=q)
    Kmat = K[(myrange[i]-time_back-shift_feature):(myrange[i]-shift_feature),]
    offset_mat = offset_mat[(myrange[i]-time_back):myrange[i],]
      
    autoencoder = fitAutoencoder(Kmat,offset_mat=offset_mat, q=q, pseudocount=pseudocount,  
                                 maxIter=maxIter, verbose=verbose)
    if(!is.null(autoencoder)) {
      hidden_layers[[i]][((myrange[i]-time_back)):(myrange[i]),] = encode(autoencoder)
    }
  }
  do.call("cbind", hidden_layers)
}

