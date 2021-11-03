


#install.packages("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod_final_benchmark/dod/", repos=NULL, type="source")

library(dod)
library(testthat)

data(salBerlin_new)

## Extract features from autoencoder
Kmat = do.call("cbind", lapply(salBerlin_new, function(x) x@observed))
ann_features = extractFeatures(Kmat, myrange=342)


## Create models
## Negative Binomial
# Harmonic
dod_har_nb = DODmodel(DODfamily("NegBinom"),
                      DODformula("Harmonic"))
# Farrington-Noufaily
dod_fn_nb = DODmodel(DODfamily("NegBinom"),
                     DODformula("FarringtonNoufaily", w=4))
# Autoencoder features
dod_extdata_nb = DODmodel(DODfamily("NegBinom"),
                          DODformula("ExtData", extdata=data.frame(h1=ann_features)))
## Poisson
# Harmonic
dod_har_pois = DODmodel(DODfamily("Poisson"),
                      DODformula("Harmonic"))
# Farrington-Noufaily
dod_fn_pois = DODmodel(DODfamily("Poisson"),
                     DODformula("FarringtonNoufaily", w=4))
# Autoencoder features
dod_extdata_pois = DODmodel(DODfamily("Poisson"),
                            DODformula("ExtData", extdata=data.frame(h1=ann_features)))
## ZIPoisson
# Harmonic
dod_har_zipois = DODmodel(DODfamily("ZIPoisson"),
                             DODformula("Harmonic"))
# Farrington-Noufaily
dod_fn_zipois = DODmodel(DODfamily("ZIPoisson"),
                       DODformula("FarringtonNoufaily", w=4))
# Autoencoder features
dod_extdata_zipois = DODmodel(DODfamily("ZIPoisson"),
                            DODformula("ExtData", extdata=data.frame(h1=ann_features)))
## ZINegBinom
# Harmonic
dod_har_zinb = DODmodel(DODfamily("ZINegBinom"),
                               DODformula("Harmonic"))
# Farrington-Noufaily
dod_fn_zinb = DODmodel(DODfamily("ZINegBinom"),
                               DODformula("FarringtonNoufaily", w=4))
# Autoencoder features
dod_extdata_zinb = DODmodel(DODfamily("ZINegBinom"),
                            DODformula("ExtData", extdata=data.frame(h1=ann_features)))
                              
                           

single_ts = salBerlin_new[[1]]
multiple_ts = salBerlin_new[1:2]

## Test models
## Negative Binomial
# Harmonic
res_har_single_nb = list(dod(single_ts, dod_har_nb, 342, learning_type = "unsupervised"),
                      dod(single_ts, dod_har_nb, 342, learning_type = "semisupervised"),
                      dod(single_ts, dod_har_nb, 342, learning_type = "supervised"))
res_har_mult_nb = list(dod(multiple_ts, dod_har_nb, 342, learning_type = "unsupervised"),
                    dod(multiple_ts, dod_har_nb, 342, learning_type = "semisupervised"),
                    dod(multiple_ts, dod_har_nb, 342, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single_nb = list(dod(single_ts, dod_fn_nb, 342, learning_type = "unsupervised"),
                     dod(single_ts, dod_fn_nb, 342, learning_type = "semisupervised"),
                     dod(single_ts, dod_fn_nb, 342, learning_type = "supervised"))
res_fn_mult_nb = list(dod(multiple_ts, dod_fn_nb, 342, learning_type = "unsupervised"),
                   dod(multiple_ts, dod_fn_nb, 342, learning_type = "semisupervised"),
                   dod(multiple_ts, dod_fn_nb, 342, learning_type = "supervised"))
# CAFE
res_extdata_single_nb = list(dod(single_ts, dod_extdata_nb, 342, learning_type = "unsupervised"),
                          dod(single_ts, dod_extdata_nb, 342, learning_type = "semisupervised"),
                          dod(single_ts, dod_extdata_nb, 342, learning_type = "supervised"))
res_extdata_mult_nb = list(dod(multiple_ts, dod_extdata_nb, 342, learning_type = "unsupervised"),
                        dod(multiple_ts, dod_extdata_nb, 342, learning_type = "semisupervised"),
                        dod(multiple_ts, dod_extdata_nb, 342, learning_type = "supervised"))

## Poisson
# Harmonic
res_har_single_pois = list(dod(single_ts, dod_har_pois, 342, learning_type = "unsupervised"),
                         dod(single_ts, dod_har_pois, 342, learning_type = "semisupervised"),
                         dod(single_ts, dod_har_pois, 342, learning_type = "supervised"))
res_har_mult_pois = list(dod(multiple_ts, dod_har_pois, 342, learning_type = "unsupervised"),
                       dod(multiple_ts, dod_har_pois, 342, learning_type = "semisupervised"),
                       dod(multiple_ts, dod_har_pois, 342, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single_pois = list(dod(single_ts, dod_fn_pois, 342, learning_type = "unsupervised"),
                        dod(single_ts, dod_fn_pois, 342, learning_type = "semisupervised"),
                        dod(single_ts, dod_fn_pois, 342, learning_type = "supervised"))
res_fn_mult_pois = list(dod(multiple_ts, dod_fn_pois, 342, learning_type = "unsupervised"),
                      dod(multiple_ts, dod_fn_pois, 342, learning_type = "semisupervised"),
                      dod(multiple_ts, dod_fn_pois, 342, learning_type = "supervised"))
# CAFE
res_extdata_single_pois = list(dod(single_ts, dod_extdata_pois, 342, learning_type = "unsupervised"),
                             dod(single_ts, dod_extdata_pois, 342, learning_type = "semisupervised"),
                             dod(single_ts, dod_extdata_pois, 342, learning_type = "supervised"))
res_extdata_mult_pois = list(dod(multiple_ts, dod_extdata_pois, 342, learning_type = "unsupervised"),
                           dod(multiple_ts, dod_extdata_pois, 342, learning_type = "semisupervised"),
                           dod(multiple_ts, dod_extdata_pois, 342, learning_type = "supervised"))

## ZIPoisson
# Harmonic
res_har_single_zipois = list(dod(single_ts, dod_har_zipois, 342, learning_type = "unsupervised"),
                           dod(single_ts, dod_har_zipois, 342, learning_type = "semisupervised"),
                           dod(single_ts, dod_har_zipois, 342, learning_type = "supervised"))
res_har_mult_zipois = list(dod(multiple_ts, dod_har_zipois, 342, learning_type = "unsupervised"),
                         dod(multiple_ts, dod_har_zipois, 342, learning_type = "semisupervised"),
                         dod(multiple_ts, dod_har_zipois, 342, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single_zipois = list(dod(single_ts, dod_fn_zipois, 342, learning_type = "unsupervised"),
                          dod(single_ts, dod_fn_zipois, 342, learning_type = "semisupervised"),
                          dod(single_ts, dod_fn_zipois, 342, learning_type = "supervised"))
res_fn_mult_zipois = list(dod(multiple_ts, dod_fn_zipois, 342, learning_type = "unsupervised"),
                        dod(multiple_ts, dod_fn_zipois, 342, learning_type = "semisupervised"),
                        dod(multiple_ts, dod_fn_zipois, 342, learning_type = "supervised"))
# CAFE
res_extdata_single_zipois = list(dod(single_ts, dod_extdata_zipois, 342, learning_type = "unsupervised"),
                               dod(single_ts, dod_extdata_zipois, 342, learning_type = "semisupervised"),
                               dod(single_ts, dod_extdata_zipois, 342, learning_type = "supervised"))
res_extdata_mult_zipois = list(dod(multiple_ts, dod_extdata_zipois, 342, learning_type = "unsupervised"),
                             dod(multiple_ts, dod_extdata_zipois, 342, learning_type = "semisupervised"),
                             dod(multiple_ts, dod_extdata_zipois, 342, learning_type = "supervised"))

## ZINegBinom
# Harmonic
res_har_single_zinb = list(dod(single_ts, dod_har_zinb, 342, learning_type = "unsupervised"),
                             dod(single_ts, dod_har_zinb, 342, learning_type = "semisupervised"),
                             dod(single_ts, dod_har_zinb, 342, learning_type = "supervised"))
res_har_mult_zinb = list(dod(multiple_ts, dod_har_zinb, 342, learning_type = "unsupervised"),
                           dod(multiple_ts, dod_har_zinb, 342, learning_type = "semisupervised"),
                           dod(multiple_ts, dod_har_zinb, 342, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single_zinb = list(dod(single_ts, dod_fn_zinb, 342, learning_type = "unsupervised"),
                            dod(single_ts, dod_fn_zinb, 342, learning_type = "semisupervised"),
                            dod(single_ts, dod_fn_zinb, 342, learning_type = "supervised"))
res_fn_mult_zinb = list(dod(multiple_ts, dod_fn_zinb, 342, learning_type = "unsupervised"),
                          dod(multiple_ts, dod_fn_zinb, 342, learning_type = "semisupervised"),
                          dod(multiple_ts, dod_fn_zinb, 342, learning_type = "supervised"))
# CAFE
res_extdata_single_zinb = list(dod(single_ts, dod_extdata_zinb, 342, learning_type = "unsupervised"),
                                 dod(single_ts, dod_extdata_zinb, 342, learning_type = "semisupervised"),
                                 dod(single_ts, dod_extdata_zinb, 342, learning_type = "supervised"))
res_extdata_mult_zinb = list(dod(multiple_ts, dod_extdata_zinb, 342, learning_type = "unsupervised"),
                               dod(multiple_ts, dod_extdata_zinb, 342, learning_type = "semisupervised"),
                               dod(multiple_ts, dod_extdata_zinb, 342, learning_type = "supervised"))


if(FALSE) {
  results = list(res_har_single_nb=res_har_single_nb,
                 res_har_mult_nb=res_har_mult_nb,
                 res_fn_single_nb=res_fn_single_nb,
                 res_fn_mult_nb=res_fn_mult_nb,
                 res_extdata_single_nb=res_extdata_single_nb,
                 res_extdata_mult_nb=res_extdata_mult_nb,
                 
                 res_har_single_pois=res_har_single_pois,
                 res_har_mult_pois=res_har_mult_pois,
                 res_fn_single_pois=res_fn_single_pois,
                 res_fn_mult_pois=res_fn_mult_pois,
                 res_extdata_single_pois=res_extdata_single_pois,
                 res_extdata_mult_pois=res_extdata_mult_pois,
                 
                 res_har_single_zipois=res_har_single_zipois,
                 res_har_mult_zipois=res_har_mult_zipois,
                 res_fn_single_zipois=res_fn_single_zipois,
                 res_fn_mult_zipois=res_fn_mult_zipois,
                 res_extdata_single_zipois=res_extdata_single_zipois,
                 res_extdata_mult_zipois=res_extdata_mult_zipois,
                 
                 res_har_single_zinb=res_har_single_zinb,
                 res_har_mult_zinb=res_har_mult_zinb,
                 res_fn_single_zinb=res_fn_single_zinb,
                 res_fn_mult_zinb=res_fn_mult_zinb,
                 res_extdata_single_zinb=res_extdata_single_zinb,
                 res_extdata_mult_zinb=res_extdata_mult_zinb)
}
#save(results, file="S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/data/results.rda")

load("C:/Users/zacherb/Desktop/dod_paper_package/Statistik.dod/data/results.rda")

#load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod_final_benchmark/dod/data/results.rda")
test_that("Consistensy of results", {
  expect_equal(res_har_single_nb, results[["res_har_single_nb"]])
  expect_equal(res_har_mult_nb, results[["res_har_mult_nb"]])
  expect_equal(res_fn_single_nb, results[["res_fn_single_nb"]])
  expect_equal(res_fn_mult_nb, results[["res_fn_mult_nb"]])
  expect_equal(res_extdata_single_nb, results[["res_extdata_single_nb"]])
  expect_equal(res_extdata_mult_nb, results[["res_extdata_mult_nb"]])
  
  expect_equal(res_har_single_pois, results[["res_har_single_pois"]])
  expect_equal(res_har_mult_pois, results[["res_har_mult_pois"]])
  expect_equal(res_fn_single_pois, results[["res_fn_single_pois"]])
  expect_equal(res_fn_mult_pois, results[["res_fn_mult_pois"]])
  expect_equal(res_extdata_single_pois, results[["res_extdata_single_pois"]])
  expect_equal(res_extdata_mult_pois, results[["res_extdata_mult_pois"]])
  
  expect_equal(res_har_single_zipois, results[["res_har_single_zipois"]])
  expect_equal(res_har_mult_zipois, results[["res_har_mult_zipois"]])
  expect_equal(res_fn_single_zipois, results[["res_fn_single_zipois"]])
  expect_equal(res_fn_mult_zipois, results[["res_fn_mult_zipois"]])
  expect_equal(res_extdata_single_zipois, results[["res_extdata_single_zipois"]])
  expect_equal(res_extdata_mult_zipois, results[["res_extdata_mult_zipois"]])
  
  expect_equal(res_har_single_zinb, results[["res_har_single_zinb"]])
  expect_equal(res_har_mult_zinb, results[["res_har_mult_zinb"]])
  expect_equal(res_fn_single_zinb, results[["res_fn_single_zinb"]])
  expect_equal(res_fn_mult_zinb, results[["res_fn_mult_zinb"]])
  expect_equal(res_extdata_single_zinb, results[["res_extdata_single_zinb"]])
  expect_equal(res_extdata_mult_zinb, results[["res_extdata_mult_zinb"]])
})



