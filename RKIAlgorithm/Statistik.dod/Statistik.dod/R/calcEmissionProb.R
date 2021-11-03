setGeneric("calcEmissionProb", function(distribution, modelData) standardGeneric("calcEmissionProb"))

setMethod("calcEmissionProb", signature = c("Poisson", "data.frame"),
          function(distribution, modelData) {
            prob = dpois(modelData$response,lambda=modelData$mu)
            emissionProb = cbind(prob[modelData$state==0], prob[modelData$state==1])
            emissionProb
          })

setMethod("calcEmissionProb", signature = c("NegBinom", "data.frame"),
          function(distribution, modelData) {
            prob = dnbinom(modelData$response, mu=modelData$mu, size=modelData$dispersion)
            emissionProb = cbind(prob[modelData$state==0], prob[modelData$state==1])
            emissionProb
          })

dzipois = function (x, lambda, pi) {
  pi*ifelse(x==0,1,0) + (1-pi)*(dpois(x, lambda=lambda))
}

setMethod("calcEmissionProb", signature = c("ZIPoisson", "data.frame"),
          function(distribution, modelData) {
            prob = dzipois(modelData$response,lambda=modelData$mu,pi=modelData$pi)
            emissionProb = cbind(prob[modelData$state==0], prob[modelData$state==1])
            emissionProb
          })

dzinbinom = function (x, mu, size, pi) {
  pi*ifelse(x==0,1,0) + (1-pi)*dnbinom(x, mu=mu, size=size)
}

setMethod("calcEmissionProb", signature = c("ZINegBinom", "data.frame"),
          function(distribution, modelData) {
            prob = dzinbinom(modelData$response,mu=modelData$mu,pi=modelData$pi,size=modelData$dispersion)
            emissionProb = cbind(prob[modelData$state==0], prob[modelData$state==1])
            emissionProb
          })



