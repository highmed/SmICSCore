
setwd("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/R/new")

load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/data/stsObjects_2001_2017_all.rda")
load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/data/sal_feature.rda")
library(surveillance)
library(MASS)
source("DistrFamily-class.R")  
source("ModelStruct-class.R")   
source("Emission-class.R")      
source("HMM-class.R")           
source("assign.weights.R")
source("updateEmissionGLMNegBinom.R") 
source("forwardBackward.R")    
source("prepareData.R")        
source("initGLM.R")
source("utils.R")
source("calculatePvalue.R")
source("dod.R")
source("calcEmissionProb.R")    
source("fitSupervised.R")
source("fitUnsupervised.R")
source("autoencoder_nb.R")


library(testthat)


## Create models
# Harmonic
emission_harmonic = EmissionGLM(DistrFamily("NegBinom"),
                                ModelStruct("Harmonic"),
                                model_type="glm")
hmm_har_nb = HMM(emission_harmonic)
emission_harmonic_shared = EmissionGLM(DistrFamily("NegBinom", sharedModel=F, sharedDispersion=F),
                                ModelStruct("Harmonic"),
                                model_type="glm")
hmm_har_nb_shared = HMM(emission_harmonic_shared)
# Farrington-Noufaily
emission_fn = EmissionGLM(DistrFamily("NegBinom"),
                                ModelStruct("FarringtonNoufaily"),
                                model_type="glm")
hmm_fn_nb = HMM(emission_fn)
emission_fn_shared = EmissionGLM(DistrFamily("NegBinom", sharedModel=F, sharedDispersion=F),
                                       ModelStruct("FarringtonNoufaily"),
                                       model_type="glm")
hmm_fn_nb_shared = HMM(emission_fn_shared)
# Autoencoder features
emission_extdata = EmissionGLM(DistrFamily("NegBinom"),
                          ModelStruct("ExtData", extdata=data.frame(h1=sal_feature)),
                          model_type="glm")
hmm_extdata_nb = HMM(emission_extdata)
emission_extdata_shared = EmissionGLM(DistrFamily("NegBinom", sharedModel=F, sharedDispersion=F),
                                 ModelStruct("ExtData", extdata=data.frame(h1=sal_feature)),
                                 model_type="glm")
hmm_extdata_nb_shared = HMM(emission_extdata_shared)


single_ts = stsObjects$SAL$`SK Berlin Reinickendorf`
multiple_ts = stsObjects$SAL[grep("Berlin", names(stsObjects$SAL))]

## Test models
# Harmonic
res_har_single = list(dod(single_ts, hmm_har_nb, 816, learning_type = "unsupervised"),
      dod(single_ts, hmm_har_nb, 816, learning_type = "semisupervised"),
      dod(single_ts, hmm_har_nb, 816, learning_type = "supervised"))
res_har_mult = list(dod(multiple_ts, hmm_har_nb_shared, 816, learning_type = "unsupervised"),
                     dod(multiple_ts, hmm_har_nb_shared, 816, learning_type = "semisupervised"),
                     dod(multiple_ts, hmm_har_nb_shared, 816, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single = list(dod(single_ts, hmm_fn_nb, 816, learning_type = "unsupervised"),
                      dod(single_ts, hmm_fn_nb, 816, learning_type = "semisupervised"),
                      dod(single_ts, hmm_fn_nb, 816, learning_type = "supervised"))
res_fn_mult = list(dod(multiple_ts, hmm_fn_nb_shared, 816, learning_type = "unsupervised"),
                     dod(multiple_ts, hmm_fn_nb_shared, 816, learning_type = "semisupervised"),
                     dod(multiple_ts, hmm_fn_nb_shared, 816, learning_type = "supervised"))
# CAFE
res_extdata_single = list(dod(single_ts, hmm_extdata_nb, 816, learning_type = "unsupervised"),
                     dod(single_ts, hmm_extdata_nb, 816, learning_type = "semisupervised"),
                     dod(single_ts, hmm_extdata_nb, 816, learning_type = "supervised"))
res_extdata_mult = list(dod(multiple_ts, hmm_extdata_nb_shared, 816, learning_type = "unsupervised"),
                    dod(multiple_ts, hmm_extdata_nb_shared, 816, learning_type = "semisupervised"),
                    dod(multiple_ts, hmm_extdata_nb_shared, 816, learning_type = "supervised"))


load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/data/results.rda")
test_that("Consistensy of results", {
  expect_equal(res_har_single, results[["res_har_single"]])
  expect_equal(res_har_mult, results[["res_har_mult"]])
  expect_equal(res_fn_single, results[["res_fn_single"]])
  expect_equal(res_fn_mult, results[["res_fn_mult"]])
  expect_equal(res_extdata_single, results[["res_extdata_single"]])
  expect_equal(res_extdata_mult, results[["res_extdata_mult"]])
})


#results = list(res_har_single=res_har_single,
#               res_har_mult=res_har_mult,
#               res_fn_single=res_fn_single,
#               res_fn_mult=res_fn_mult,
#               res_extdata_single=res_extdata_single,
#               res_extdata_mult=res_extdata_mult)

#save(results, file="S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/data/results.rda")
