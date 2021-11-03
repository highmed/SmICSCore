
setGeneric("createFormula", function(distribution, dod_formula) standardGeneric("createFormula"))

setMethod("createFormula", signature = c("DODfamily", "FarringtonNoufaily"),
          function(distribution, dod_formula) {
            
            params = c("1", ifelse(dod_formula@timeTrend, "wtime", ""), "seasgroups")
            
            # Add shared model specification and collapse
            if(!dod_formula@shared_params) {
              params[params=="1"] = "id"
              params[params!="id"] = paste(params[params!="id"], "*id", sep="")
            }
            params[length(params)+1] = "offset(log(population))"
            params = paste(params, collapse=" + ")
            
            paste0("response ~ ", params)
            
          })


setMethod("createFormula", signature = c("DODfamily", "Harmonic"),
          function(distribution, dod_formula) {
            
            season = NULL
            if(dod_formula@S > 0) {
              season = paste0(c("sin", "cos"), dod_formula@S)
            }
            wtime = NULL
            if(dod_formula@timeTrend) {
              wtime = "wtime"
            }
            params = c("1", wtime, season)
            
            # Add shared model specification and collapse
            if(!dod_formula@shared_params) {
              params[params=="1"] = "id"
              params[params!="id"] = paste(params[params!="id"], "*id", sep="")
            }
            params[length(params)+1] = "offset(log(population))"
            params = paste(params, collapse=" + ")
            
            paste0("response ~ ", params)
            
          })

setMethod("createFormula", signature = c("DODfamily", "ExtData"),
          function(distribution, dod_formula) {
            
            params = c("1", names(dod_formula@extdata))
            
            # Add shared model specification and collapse
            if(!dod_formula@shared_params) {
              params[params=="1"] = "id"
              params[params!="id"] = paste(params[params!="id"], "*id", sep="")
            }
            params[length(params)+1] = "offset(log(population))"
            params = paste(params, collapse=" + ")
            
            paste0("response ~ ", params)
            
          })
