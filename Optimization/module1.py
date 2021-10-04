def return_list(hps,params):
    return hps + params

hpsin = [1,2,3,4,5,6,7]
paramsin = [4,5,6]

print(return_list(hpsin, paramsin))

print(hpsin[1:4]) # [2, 3, 4] including index 1 to excluding index 4
