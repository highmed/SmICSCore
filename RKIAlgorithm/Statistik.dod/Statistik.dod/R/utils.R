
log_dirichlet <- function(x, alpha) {
  logD <- sum(lgamma(alpha)) - lgamma(sum(alpha))
  s <- sum((alpha - 1) * log(x))
  (sum(s) - logD)
}

mpow <- function(P, n) {
  if (n == 0) diag(nrow(P))
  else if (n == 1) P
  else P %*% mpow(P, n - 1)
}
