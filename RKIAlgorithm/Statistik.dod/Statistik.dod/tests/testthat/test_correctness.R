
#install.packages("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/", repos=NULL, type="source")
load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/data/stsObjects_2001_2017_all.rda")
load("S:/OE/FG37/Zacher/Projekte/dod_paper_package/dod/data/sal_feature.rda")

library(dod)
library(testthat)


## Create models
# Harmonic
dod_har_nb = DODmodel(DODfamily("NegBinom", shared_dispersion=TRUE),
                      DODformula("Harmonic", shared_params=TRUE))
dod_har_nb_shared = DODmodel(DODfamily("NegBinom"),
                             DODformula("Harmonic"))
# Farrington-Noufaily
dod_fn_nb = DODmodel(DODfamily("NegBinom", shared_dispersion=TRUE),
                     DODformula("FarringtonNoufaily", shared_params=TRUE))
dod_fn_nb_shared = DODmodel(DODfamily("NegBinom"),
                            DODformula("FarringtonNoufaily"))
# Autoencoder features
dod_extdata_nb = DODmodel(DODfamily("NegBinom", shared_dispersion=TRUE),
                          DODformula("ExtData", extdata=data.frame(h1=sal_feature),
                                     shared_params=TRUE))
dod_extdata_nb_shared = DODmodel(DODfamily("NegBinom"),
                                 DODformula("ExtData", extdata=data.frame(h1=sal_feature)))


single_ts = stsObjects$SAL$`SK Berlin Reinickendorf`
multiple_ts = stsObjects$SAL[grep("Berlin", names(stsObjects$SAL))]

## Test models
# Harmonic
res_har_single = list(dod(single_ts, dod_har_nb, 816, learning_type = "unsupervised"),
      dod(single_ts, dod_har_nb, 816, learning_type = "semisupervised"),
      dod(single_ts, dod_har_nb, 816, learning_type = "supervised"))
res_har_mult = list(dod(multiple_ts, dod_har_nb_shared, 816, learning_type = "unsupervised"),
                     dod(multiple_ts, dod_har_nb_shared, 816, learning_type = "semisupervised"),
                     dod(multiple_ts, dod_har_nb_shared, 816, learning_type = "supervised"))
# Farrington Noufaily
res_fn_single = list(dod(single_ts, dod_fn_nb, 816, learning_type = "unsupervised"),
                      dod(single_ts, dod_fn_nb, 816, learning_type = "semisupervised"),
                      dod(single_ts, dod_fn_nb, 816, learning_type = "supervised"))
res_fn_mult = list(dod(multiple_ts, dod_fn_nb_shared, 816, learning_type = "unsupervised"),
                     dod(multiple_ts, dod_fn_nb_shared, 816, learning_type = "semisupervised"),
                     dod(multiple_ts, dod_fn_nb_shared, 816, learning_type = "supervised"))
# CAFE
res_extdata_single = list(dod(single_ts, dod_extdata_nb, 816, learning_type = "unsupervised"),
                     dod(single_ts, dod_extdata_nb, 816, learning_type = "semisupervised"),
                     dod(single_ts, dod_extdata_nb, 816, learning_type = "supervised"))
res_extdata_mult = list(dod(multiple_ts, dod_extdata_nb_shared, 816, learning_type = "unsupervised"),
                    dod(multiple_ts, dod_extdata_nb_shared, 816, learning_type = "semisupervised"),
                    dod(multiple_ts, dod_extdata_nb_shared, 816, learning_type = "supervised"))


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
