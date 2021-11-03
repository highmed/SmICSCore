


#' Calculate the log likelihood of a weighted negative binomial regression
#' 
#' @title Log likelihood of a weighted negative binomial regression
#' 
#' @param theta Dispersion parameter of the negative binomial distribution.
#' @param mu Mean of the negative binomial distribution.
#' @param y A vector of counts.
#' @param omega Regression weights.
#' 
#' @usage glmLogLikNB(theta, mu, y, omega)
#' 
#' @keywords internal
#' @noRd
glmLogLikNB <- function(theta, mu, y, omega) {
  sum(omega*dnbinom(y,size=theta,mu=mu,log=TRUE))
} 


#' Calculate the gradient of the dispersion parameter in a weighted negative binomial regression
#' 
#' @title Gradient of the dispersion parameter
#' 
#' @param par Dispersion parameter of the negative binomial distribution.
#' @param mu Mean of the negative binomial distribution.
#' @param y A vector of counts.
#' @param omega Regression weights.
#' 
#' @usage gradThetaNB(par, y, mu, omega)
#' 
#' @keywords internal
#' @noRd
gradThetaNB = function(par, y, mu, omega) {
  theta = exp(par)
  gradTheta <- sum(omega * theta * 
                     (digamma(y + theta) - digamma(theta) + 
                        log(theta) - log(mu + theta) + 1 - 
                        (y + theta)/(mu + theta)))
  -gradTheta
}


#' Calculate the negative log likelihood of a weighted negative binomial regression
#' 
#' @title Negative log likelihood of a weighted negative binomial regression
#' 
#' @param par Log of the dispersion parameter of the negative binomial distribution.
#' @param mu Mean of the negative binomial distribution.
#' @param y A vector of counts.
#' @param omega Regression weights.
#' 
#' @usage negLogLikNB(par, y, mu, omega)
#' 
#' @keywords internal
#' @noRd
negLogLikNB = function(par, y, mu, omega) {
  theta = exp(par)
  - glmLogLikNB(theta, mu, y, omega)
  
}


#' Maximum likelihood estimation of the dispersion parameter of a negative binomial regression
#' 
#' @title Maximum likelihood estimation of the dispersion parameter of a negative binomial regression
#' 
#' @param thetas Dispersion parameters of the negative binomial distribution.
#' @param id2index A list containing the mapping of each element in thetas to the indices of the case counts y.
#' @param glm_fitted Fitte glm, output of glm.fit.
#' @param y A vector of case counts.
#' @param omega A vector containing regression weights.
#' 
#' @usage estimateTheta(thetas, id2index, glm_fitted, y, omega)
#' 
#' @return A list containing \itemize{
#'    \item{thetas - }{The dispersion parameters, one for each element in id2index.} 
#'    \item{all_theta - }{The dispersion parameters, one for each element in y.}
#' }
#' 
#' @keywords internal
#' @noRd
estimateTheta = function(thetas, id2index, glm_fitted, y, omega) {
  
  new_thetas = thetas
  all_theta = rep(NA, length(omega))
  
  curr_theta = NA
  for(n in names(id2index)) {
    # Extract data/variables for fitting
    curr_fitted = glm_fitted$fitted.values[id2index[[n]]]
    curr_y = y[id2index[[n]]] 
    w_curr = omega[id2index[[n]]] 
    new_ll_theta = -Inf
    old_ll_theta = Inf
    
    # Fit dispersion of negative binomial using theta.ml function from MASS package
    #suppressWarnings(old_ll_theta <- glmLogLikNB(th=thetas[n], mu=curr_fitted, y=curr_y, omega=w_curr))
    #suppressWarnings(curr_theta <- theta.ml(curr_y, curr_fitted, sum(w_curr), w_curr, limit = 25))
    suppressWarnings(curr_theta <- tryCatch(
    (theta.ml(curr_y, curr_fitted, sum(w_curr), w_curr, limit = 25)), 
    error = function(e) {
      #print(e)
       -1
    }))
    #suppressWarnings(new_ll_theta <- glmLogLikNB(th=curr_theta[1], mu=curr_fitted, y=curr_y, omega=w_curr))
  #  print("start")
#print(curr_theta)
#print(c(old_ll_theta-new_ll_theta))
    # If fitting of dispersion failed because of error or decreas in log likelihood try (L-)BFGS
    if(TRUE) {
      if(curr_theta[1] == -1 | is.na(curr_theta)) {
        # Fit dispersion using L-BFGS algorithm
        curr_theta <- tryCatch({
          suppressWarnings(exp(optim(log(thetas[n]),fn=negLogLikNB, gr=gradThetaNB, y=curr_y, mu=curr_fitted, omega=w_curr, method="L-BFGS")$par))}, 
          error = function(e) {
            -1
          })
        # Fit dispersion using BFGS algorithm if L-BFGS failed
        if(curr_theta == -1 | is.na(curr_theta)) {
          curr_theta <- tryCatch({
            suppressWarnings(exp(optim(log(thetas[n]),fn=negLogLikNB, gr=gradThetaNB, y=curr_y, mu=curr_fitted, omega=w_curr, method="BFGS")$par))}, 
            error = function(e) {
              -1
            })
        }
      }
    }
    
    
    if(curr_theta[1] == -1 | is.na(curr_theta)) {
      stop("Error fitting dispersion of Negative Binomial.\n")
    }
    
    #new_thetas[n] = min(c(1e6,max(c(1e-6,curr_theta[1]))))
    new_thetas[n] = curr_theta[1]
    #print(new_thetas)
    all_theta[id2index[[n]]] = new_thetas[n]
  }
  
  list(thetas=new_thetas, all_theta=all_theta)
}



#' Update Negative Binomial GLM parameters during EM
#' 
#' @title Update Negative Binomial GLM parameters
#' 
#' @param emission Emission object of the DODmodel parameterization.
#' @param dat A data frame containing the data for fitting.
#' @param omega Regression weights.
#' 
#' @return A data frame containing updated estimates of the mean and dispersion parameters.
#' 
#' @seealso \code{\linkS4class{EmissionGLMNegBinom}}
#' 
#' @keywords internal
#' @noRd
setMethod("updateEmission", signature = c("EmissionGLMNegBinom", "data.frame", "numeric"),
          function(emission, dat, omega) {
            
            formula = emission@dod_formula@formula
            mf = model.frame(as.formula(formula), data=dat)
            
            # Extract response, model matrix and offset
            model_offset_all = model.offset(mf)
            y = model.response(mf, type="numeric")
            modelData = model.matrix(as.formula(formula), dat)
            X = modelData
            
            # Remove data with weight==0 for faster and more stable fitting
            non_zero_weights = which(omega != 0)
            modelData = modelData[non_zero_weights,]
            model_offset = model_offset_all[non_zero_weights]
            y = y[non_zero_weights]
            omega_temp <- omega[non_zero_weights]
            dat_orig = dat
            dat = dat[non_zero_weights,]
            
            # Create mapping for time series sharing the same dispersion parameters to indices in data matrix
            id2index_all = tapply(1:nrow(dat_orig), INDEX=dat_orig$shared_dispersion, identity)
            id2index = tapply(1:nrow(dat), INDEX=dat$shared_dispersion, identity)
            allIds = unique(dat$shared_dispersion)
            
            if(TRUE) {
              # Estimate initial value for dispersion
              suppressWarnings(fit <- glm.fit(x=modelData, y=y, family = poisson(link="log"), 
                                                     weights=omega_temp, offset=model_offset))
              
              thetas = rep(1, length(allIds))
              names(thetas) = allIds
              for(n in names(id2index)) {
                curr_y = y[id2index[[n]]] 
                w_curr = omega_temp[id2index[[n]]] 
                curr_mu = fit$fitted.values[id2index[[n]]]
                theta_init <- sum(w_curr)/sum(w_curr*(curr_y/curr_mu-1)^2)
                thetas[n] = max(c(theta_init,0.01))

              }
              #print(thetas)
              theta.est = estimateTheta(thetas, id2index, fit, y, omega_temp)
              thetas = theta.est$thetas
              
              all_theta = rep(NA, nrow(dat))
              for(n in names(id2index)) {
                all_theta[id2index[[n]]] = thetas[n]
              }
              fam = negative.binomial(theta=all_theta)
              
  
              iter = 0
              denom1 = sqrt(2 * max(1, fit$df.residual))
              denom2 = 1 
              theta_diff = 1
              # print(all_theta)
              mu = fit$fitted.values
              new_loglik = glmLogLikNB(th=all_theta, mu=mu, y=y, omega=omega_temp)
              old_loglik = new_loglik + 2 * denom1
              curr_theta = NA
              
              while ((iter <- iter + 1) <= 25 && (abs(old_loglik - new_loglik)/denom1 + abs(theta_diff)/denom2) > 1e-08) {
                # Estimate theta
                old_thetas = thetas
                theta.est = estimateTheta(thetas, id2index, fit, y, omega_temp)
                thetas = theta.est$thetas
                all_theta = theta.est$all_theta
                
                
                # Estimate glm parameters
                eta <- fam$linkfun(mu)
                suppressWarnings(fit <- glm.fit(x=modelData, y=y, w=omega_temp, 
                                                etastart=eta, family=fam, 
                                                offset=model_offset))
                
                #theta1 = theta.ml(y, mu, n = sum(omega_temp), omega_temp, limit = 25)[1]
                #print(c(thetas, theta1))
                
                #names(thetas) = allIds
                #print(thetas)
                mu = fit$fitted.values
                
                  
                # Compute updated log likelihood and updated parameters
                fam = negative.binomial(theta=all_theta)
                theta_diff = max(abs(old_thetas - thetas))
                old_loglik = new_loglik
                new_loglik = glmLogLikNB(th=all_theta, mu=mu, y=y, omega=omega_temp)
                #print(glmLogLikNB(th=all_theta, mu=y, y=y, omega=omega_temp)-new_loglik)
              }
            }
            
            #form = as.formula(formula)
            #suppressWarnings(fit <- glm.nb(form, data=dat, weights=omega_temp, control=glm.control(trace=FALSE)))
            #thetas = fit$theta
           # print(c(thetas, theta1))
            #names(thetas) = allIds
            
            emission@dod_formula@coefficients = fit$coefficients
            emission@distribution@dispersion = thetas
            
            # Calclulate updated mean and dispersion values for all time series
            mu = exp(X %*% fit$coefficients + model_offset_all)
            all_theta = rep(NA, nrow(X))
            for(n in names(id2index)) {
              all_theta[id2index_all[[n]]] = thetas[n]
            }
            
            dat_orig$mu=mu
            dat_orig$dispersion=all_theta
            list(model=dat_orig, emission=emission)
            
          })




