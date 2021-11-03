

setGeneric("calculatePvalue", function(distribution, result) standardGeneric("calculatePvalue"))

setMethod("calculatePvalue", signature = c("NegBinom", "data.frame"),
          function(distribution, result) {
            pnbinom(q=result$observed-1,
                    size=result$dispersion, 
                    mu=result$mu0, 
                    lower.tail=FALSE)
          })

setMethod("calculatePvalue", signature = c("Poisson", "data.frame"),
          function(distribution, result) {
            ppois(q=result$observed-1,
                    lambda=result$mu0, 
                    lower.tail=FALSE)
          })


pzipois = function (q, lambda, pi) {
  ifelse(q<0, 0, pi + (1 - pi) * ppois(q, lambda=lambda))
}

setMethod("calculatePvalue", signature = c("ZIPoisson", "data.frame"),
          function(distribution, result) {
            1-as.vector(pzipois(q=result$observed-1 ,pi=result$pi,lambda=result$mu0))
          })


pzinbinom = function (q, mu, size, pi) {
  ifelse(q<0, 0, pi + (1 - pi) * pnbinom(q, mu=mu, size=size))
}

setMethod("calculatePvalue", signature = c("ZINegBinom", "data.frame"),
          function(distribution, result) {
            1-as.vector(pzinbinom(q=result$observed-1, 
                                  mu=result$mu0, 
                                  pi=result$pi,
                                  size=result$dispersion))
          })



