
#' Update Poisson GLM parameters during EM
#' 
#' @title Update Poisson GLM parameters
#' 
#' @param emission Emission object of the DODmodel parameterization.
#' @param dat A data frame containing the data for fitting.
#' @param omega Regression weights.
#' 
#' @return A data frame containing updated estimates of the mean and dispersion parameters.
#' 
#' @seealso \code{\linkS4class{EmissionGLMPoisson}}
#' 
#' @keywords internal
#' @noRd
setMethod("updateEmission", signature = c("EmissionGLMPoisson", "data.frame", "numeric"),
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
            
            etastart = NULL
            if(FALSE & ! any(is.na(emission@dod_formula@coefficients))) {
              etastart = modelData %*% emission@dod_formula@coefficients + model_offset
            }
            
            suppressWarnings(fit <- glm.fit(x=modelData, y=y, 
                                            etastart=etastart,
                                            family = poisson(link="log"), 
                                            weights=omega_temp))
            
            emission@dod_formula@coefficients = fit$coefficients
            # Calclulate updated mean and dispersion values for all time series
            mu = exp(X %*% fit$coefficients + model_offset_all)
            dat_orig$mu=mu
            list(model=dat_orig, emission=emission)
            
          })
            