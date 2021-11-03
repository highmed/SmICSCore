

#' This class is a generic container for the model used for outbreak detection
#' 
#' @slot name Name of the emission.
#' 
#' @exportClass Emission
setClass("Emission",
         slots = c(
           name="character"
         )
)


setClass("EmissionGLMPoisson",
         contains = "Emission",
         slots = c(
           distribution="Poisson",
           dod_formula="DODformula"
         )
)

setClass("EmissionGLMNegBinom",
         contains = "Emission",
         slots = c(
           distribution="NegBinom",
           dod_formula="DODformula"
         )
)

setClass("EmissionGLMZIPoisson",
         contains = "Emission",
         slots = c(
           distribution="ZIPoisson",
           dod_formula="DODformula"
         )
)

setClass("EmissionGLMZINegBinom",
         contains = "Emission",
         slots = c(
           distribution="ZINegBinom",
           dod_formula="DODformula"
         )
)


Emission = function(distribution, dod_formula, model="GLM") {
  

  formula_bckg = createFormula(distribution, dod_formula)
  formula = paste0(formula_bckg, " + state")
  dod_formula@formula = formula
  dod_formula@formula_bckg = formula_bckg
  
  new(paste0("Emission", toupper(model), is(distribution)[1]), distribution=distribution,
      dod_formula=dod_formula)
  
  
}

setGeneric("updateEmission", function(emission, dat, omega) standardGeneric("updateEmission"))
