
#' This class is a generic container for a family of probability distributions
#' 
#' @slot name Name of the probability distribution.
#' @slot params Character vector of name of parameters.
#' 
#' @exportClass DODfamily
setClass("DODfamily",
         slots = c(
           name="character",
           params="character"
         )
)






#' This class is a generic container for a family of Poisson distributions
#' 
#' @exportClass Poisson
setClass("Poisson", 
         contains = "DODfamily", 
         prototype = list(
           name = "Poisson",
           params = c("mu")
         )
)

#' Prints description of Poisson object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("Poisson"), function(object) {
  cat(is(object)[1], "(", paste(object@params, collapse=", "), ")\n", sep="")
})

#' This class is a generic container for a family of Zero-Inflated Poisson distributions
#' 
#' @slot shared_pi Logical indicating whether parameter pi is shared across multiple time series.
#' 
#' @exportClass ZIPoisson
setClass("ZIPoisson", 
         contains = "DODfamily", 
         slots = c(
           shared_pi="logical",
           pi="numeric"
         ),
         prototype = list(
           name = "ZIPoisson",
           params = c("mu", "pi")
         )
)


#' Prints description of ZIPoisson object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("ZIPoisson"), function(object) {
  space = paste(rep(" ", 10), collapse="")
  cat(is(object)[1], "(", 
      paste(object@params, collapse=", "), ",\n", space,
      "shared_pi=", object@shared_pi, ")\n", sep="")
})


#' This class is a generic container for a family of Negative Binomial distributions
#' 
#' @slot shared_dispersion Logical indicating whether dispersion parameter is shared across multiple time series.
#' 
#' @exportClass NegBinom
setClass("NegBinom", 
         contains = "DODfamily", 
         slots = c(
           shared_dispersion="logical",
           dispersion="numeric"
         ),
         prototype = list(
           name = "NegBinom",
           params = c("mu", "dispersion")
         )
)

#' Prints description of NegBinom object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("NegBinom"), function(object) {
  space = paste(rep(" ", 9), collapse="")
  cat(is(object)[1], "(", 
      paste(object@params, collapse=", "), ",\n", space,
      "shared_dispersion=", object@shared_dispersion, ")\n", sep="")
})

#' This class is a generic container for a family of Negative Binomial distributions
#' 
#' @slot shared_dispersion Logical indicating whether dispersion parameter is shared across multiple time series.
#' @slot shared_pi Logical indicating whether parameter pi is shared across multiple time series.
#' 
#' @exportClass ZINegBinom
setClass("ZINegBinom", 
         contains = "DODfamily", 
         slots = c(
           shared_dispersion="logical",
           shared_pi="logical",
           pi="numeric",
           dispersion="numeric"
         ),
         prototype = list(
           name = "ZINegBinom",
           params = c("mu", "dispersion", "pi")
         )
)

#' Prints description of ZINegBinom object
#' 
#' @keywords internal
#' @noRd
setMethod(f = "show", signature = c("ZINegBinom"), function(object) {
  space = paste(rep(" ", 11), collapse="")
  cat(is(object)[1], "(", 
      paste(object@params, collapse=", "), ",\n", space,
      "shared_dispersion=", object@shared_dispersion,  ",\n", space,
      "shared_pi=", object@shared_pi,  ")\n", sep="")
})


#' @title Create a family of probability distributions for outbreak detection
#' 
#' @param name Name of the probability distribution that should be used. One of c("Poisson", "ZIPoisson", "NegBinom", "ZINegBinom").
#' @param shared_dispersion Logical indicating whether dispersion parameter is shared across multiple time series. Only relevant if name is one of c("NegBinom", "ZINegBinom"). 
#' @param shared_pi Logical indicating whether parameter pi is shared across multiple time series. Only relevant if name is one of c("ZIPoisson", "ZINegBinom"). 
#' 
#' @usage DODfamily(name, shared_dispersion=TRUE, shared_pi=TRUE)
#' 
#' @seealso \code{\linkS4class{DODfamily}}
#' 
#' @examples
#' 
#' TODO
#'   
#' @export
DODfamily = function(name, 
                       shared_dispersion=FALSE, 
                       shared_pi=FALSE) {
  name = as.character(name)
  shared_dispersion = as.logical(shared_dispersion)
  shared_pi = as.logical(shared_pi)
  
  obj = NULL
  
  if(name=="Poisson") {
    obj = new(name, name = name)
  }
  if(name=="ZIPoisson") {
    obj = new(name, name = name,
              shared_pi = shared_pi,
              pi = as.numeric(NA))
  }
  if(name=="NegBinom") {
    obj = new(name, name = name,
              shared_dispersion = shared_dispersion,
              dispersion = as.numeric(NA))
  }
  if(name=="ZINegBinom") {
    obj = new(name, name = name,
              shared_dispersion = shared_dispersion,
              shared_pi = shared_pi,
              dispersion = as.numeric(NA),
              pi = as.numeric(NA))
  }
  
  obj
}


setGeneric("getParamNames", function(distribution) standardGeneric("getParamNames"))

setMethod("getParamNames", signature = c("DODfamily"),
          function(distribution) {
            distribution@params
          })


setGeneric("getSharedParN", function(distribution) standardGeneric("getSharedParN"))

setMethod("getSharedParN", signature = c("NegBinom"),
          function(distribution) {
            as.numeric(distribution@shared_dispersion)
          })

setMethod("getSharedParN", signature = c("Poisson"),
          function(distribution) {
            0
          })

setMethod("getSharedParN", signature = c("ZIPoisson"),
          function(distribution) {
            as.numeric(distribution@shared_pi)
          })

setMethod("getSharedParN", signature = c("ZINegBinom"),
          function(distribution) {
            as.numeric(distribution@shared_pi)+as.numeric(distribution@shared_dispersion)
          })

