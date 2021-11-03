
#' This class is a generic container for a formula which specifies the model used for outbreak detection
#' 
#' @slot name Name of the model. Must be one of c("FarringtonNoufaily", "Harmonic", "ExtData").
#' @slot shared_params Indicates whether model parameters are shared across multiple time series.
#' 
#' @exportClass DODformula
setClass("DODformula",
         slots=c(
           name="character",
           shared_params = "logical",
           coefficients = "numeric",
           failed_optim = "numeric"
         )
)


#' This class is a container for the parameterization of the FarringtonNoufaily models.
#' 
#' @slot noPeriods Number of levels in the factor which creates bins in each year to model seasonal patterns. 
#' @slot w The number of weeks before and after the current week to include in the bin which contains the respective week in each year.
#' @slot timeTrend Indicates whether a time trend should be included in the model.
#' @slot freq Number of time units (can be e.g. weeks or months) in a year.
#' @slot offset An offset to included in the predictor during fitting.
#' @slot formula_bckg A formula which models the endemic (background) level of cases.
#' @slot formula A formula which models the endemic (background) and epidemic levels of cases.
#'   
#' @exportClass DODformula
setClass("FarringtonNoufaily",
         contains = "DODformula", 
         slots = c(
           noPeriods="numeric", 
           w="numeric", 
           timeTrend="logical", 
           freq="numeric",
           offset="logical",
           formula_bckg="character",
           formula="character"
         ),
         prototype = list(
           name = "FarringtonNoufaily"
         )
)

#' Prints description of FarringtonNoufaily object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("FarringtonNoufaily"), function(object) {
  
  space = paste(rep(" ", 19), collapse="")
  params = paste("noPeriods=", object@noPeriods, ",\n", space,
                 "w=", object@w, ",\n", space,
                 "timeTrend=", object@timeTrend, ",\n", space,
                 "freq=", object@freq, ",\n", space,
                 "offset=", object@offset, ",\n", space,
                 "shared_params=", object@shared_params, sep="")
  
  cat(is(object)[1], "(", params, ")\n", sep="")
  
})
  


#' This class is a container for the parameterization of the Harmonic models.
#' 
#' @slot S Number of oscillations during one year.
#' @slot timeTrend Indicates whether a time trend should be included in the model.
#' @slot freq Number of time units (can be e.g. weeks or months) in a year.
#' @slot offset An offset to included in the predictor during fitting.
#' @slot formula_bckg A formula which models the endemic (background) level of cases.
#' @slot formula A formula which models the endemic (background) and epidemic levels of cases.
#'   
#' @exportClass DODformula
setClass("Harmonic",
         contains = "DODformula", 
         slots = c(
           S="numeric", 
           timeTrend="logical", 
           freq="numeric",
           offset="logical",
           formula_bckg="character",
           formula="character"
         ),
         prototype = list(
           name = "Harmonic"
         )
)

#' Prints description of Harmonic object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("Harmonic"), function(object) {
  
  space = paste(rep(" ", 9), collapse="")
  params = paste("S=", object@S, ",\n", space,
                 "timeTrend=", object@timeTrend, ",\n", space,
                 "freq=", object@freq, ",\n", space,
                 "offset=", object@offset, ",\n", space,
                 "shared_params=", object@shared_params, sep="")
  
  cat(is(object)[1], "(", params, ")\n", sep="")
  
})


#' This class is a container for the parameterization using external data.
#' 
#' @slot extdata A data.frame containing variables which are used to model case counts. All variables in the data.frame will be used in the model. The data.frame has to have the same number of rows as time points in the time series.
#' @slot freq Number of time units (can be e.g. weeks or months) in a year.
#' @slot offset An offset to included in the predictor during fitting.
#' @slot formula_bckg A formula which models the endemic (background) level of cases.
#' @slot formula A formula which models the endemic (background) and epidemic levels of cases.
#'   
#' @exportClass DODformula
setClass("ExtData",
         contains = "DODformula", 
         slots = c(
           extdata="data.frame",
           freq = "numeric",
           offset="logical",
           formula_bckg="character",
           formula="character"
         ),
         prototype = list(
           name = "ExtData"
         )
)


#' Prints description of ExtData object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("ExtData"), function(object) {
  
  space = paste(rep(" ", 8), collapse="")
  
  params = c()
  for(n in names(object@extdata)) {
    params[n] = paste0(n, "=", class(object@extdata[[n]]), ",\n", space)
  }
  
  params = paste(params,
                 "freq=", object@freq, ",\n", space,
                 "offset=", object@offset, ",\n", space,
                 "shared_params=", object@shared_params, sep="")
  
  cat(is(object)[1], "(", params, ")\n", sep="")
  
})


#' @title Create a formula of the model for outbreak detection
#' 
#' @param name Name of the model. Must be one of c("FarringtonNoufaily", "Harmonic", "ExtData"). 
#' @param S Number of oscillations during one year.
#' @param freq Number of time units (can be e.g. weeks or months) in a year.
#' @param noPeriods Number of levels in the factor which creates bins in each year to model seasonal patterns. 
#' @param w The number of weeks before and after the current week to include in the bin which contains the respective week in each year.
#' @param timeTrend Indicates whether a time trend should be included in the model.
#' @param extdata A data.frame containing variables which are used to model case counts. All variables in the data.frame will be used in the model. The data.frame has to have the same number of rows as time points in the time series.
#' @param  shared_params Indicates whether model parameters are shared across multiple time series.
#' @param offset An offset to included in the predictor during fitting.
#' 
#' @usage DODformula(name, S=1, freq=52, noPeriods=10, w=3, timeTrend=TRUE, extdata=NULL, shared_params=TRUE, offset=FALSE) 
#' 
#' @seealso \code{\linkS4class{DODformula}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
DODformula = function(name, 
                       S=1, freq=52, # specific for harmonic model
                       noPeriods=10, w=3, # specific for Farrington-Noufaily model
                       timeTrend=TRUE, # specific for harmonic and Farrington-Noufaily model
                       extdata=NULL, 
                       shared_params=FALSE,
                       offset=FALSE) {
  
  name = as.character(name)
  failed_optim = as.numeric(0)
  S = as.numeric(S)
  freq = as.numeric(freq)
  noPeriods = as.numeric(noPeriods)
  w = as.numeric(w)
  shared_params = as.logical(shared_params)
  timeTrend = as.logical(timeTrend)
  if(!is.null(extdata)) {
    extdata = as.data.frame(extdata)
  }
  else if(name == "ExtData" & is.null(extdata)) {
    stop("Must provide data.frame with DODformula==", name, "\n", sep="")
  } 
  
  obj = NULL
  
  if(name=="Harmonic") {
    obj = new(name, name = name,  S=S, 
              timeTrend=timeTrend, freq=freq, 
              offset=offset, shared_params = shared_params,
              coefficients=as.numeric(NA), 
              failed_optim=failed_optim)
  }
  else if(name=="FarringtonNoufaily") {
    obj = new(name, name = name, noPeriods = noPeriods,
              w = w, timeTrend = timeTrend, freq = freq,
              offset=offset, shared_params = shared_params,
              coefficients=as.numeric(NA), 
              failed_optim=failed_optim)
  }
  else if(name=="ExtData") {
    obj = new(name, name = name, extdata = extdata, 
              freq = freq, offset=offset, shared_params = shared_params,
              coefficients=as.numeric(NA), 
              failed_optim=failed_optim)
  }
  else {
    stop("name must be on of c(Harmonic, FarringtonNoufaily, ExtData)\n")
  }
  
  obj
  
}
