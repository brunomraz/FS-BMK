"""FORMULA STUDENT BASIC MOTION AND KINEMATICS OPTIMIZATOR"""

import numpy as np
from scipy.optimize import minimize
import time
from numpy.random import uniform as rand
from numeric_sol import call_suspension_objective, Suspension
import numeric_sol as ns
import pandas as pd
import os
import multiprocessing as mp
from constraints import hps_constraints_cobyla, hps_constraints_slsqp#, hps_bounds_slsqp
import datetime
from random import uniform as runif

S = Suspension




# used as input to minimization function inside optmiziation method
def random_initial_susp():
    hps_bounds_slsqp = [
        runif(S.lca1y_lo, S.lca1y_up), runif(S.lca1z_lo, S.lca1z_up),
        runif(S.lca2y_lo, S.lca2y_up), runif(S.lca2z_lo, S.lca2z_up),
        runif(S.lca3x_lo, S.lca3x_up), runif(S.lca3y_lo, S.lca3y_up), runif(S.lca3z_lo, S.lca3z_up),
        runif(S.uca1y_lo, S.uca1y_up), runif(S.uca1z_lo, S.uca1z_up),
        runif(S.uca2y_lo, S.uca2y_up), runif(S.uca2z_lo, S.uca2z_up),
        runif(S.uca3x_lo, S.uca3x_up), runif(S.uca3y_lo, S.uca3y_up), runif(S.uca3z_lo, S.uca3z_up),
        runif(S.tr1x_lo, S.tr1x_up), runif(S.tr1y_lo, S.tr1y_up), runif(S.tr1z_lo, S.tr1z_up),
        runif(S.tr2x_lo, S.tr2x_up), runif(S.tr2y_lo, S.tr2y_up), runif(S.tr2z_lo, S.tr2z_up)
    ]

    return hps_bounds_slsqp


def optim_process_cobyla(initial_hps, shared_list):
    # global hps_constraints
    start_time = time.time()
    sol = minimize(call_suspension_objective, initial_hps,
                   constraints=hps_constraints_cobyla,
                   method='COBYLA', options={"maxiter": 2000})
    # print("obavljena optimizacija")
    end_time = time.time()

    if sol.success == True:
        print("gotovo uspjesno")
        # APPENDA  rjesenje jedne iteracije u mp.Manager.list()
        shared_list.append(Suspension.return_hps_and_parameters() + ["COBYLA", end_time - start_time])

    else:
        print("nisu constraintovi pogodeni")


def optim_process_slsqp(initial_hps, shared_list):
    # global hps_constraints
    start_time = time.time()
    sol = minimize(call_suspension_objective, initial_hps,
                   constraints=hps_constraints_slsqp, bounds=hps_bounds_slsqp,
                   method='SLSQP', options={"maxiter": 200})
    # print("obavljena optimizacija")
    end_time = time.time()

    if sol.success == True:
        print("gotovo uspjesno")
        # APPENDA  rjesenje jedne iteracije u mp.Manager.list()
        #shared_list.append(np.append(call_S_check(sol.x)[3], ("SLSQP", end_time - start_time)))

    else:
        print("nisu constraintovi pogodeni")


# print(optim_process_cobyla(random_initial_susp()[0])[0])
column_names = [
    "LCA1 X", "LCA1 Y", "LCA1 Z", "LCA2 X", "LCA2 Y", "LCA2 Z", "LCA3 X", "LCA3 Y", "LCA3 Z",
    "UCA1 X", "UCA1 Y", "UCA1 Z", "UCA2 X", "UCA2 Y", "UCA2 Z", "UCA3 X", "UCA3 Y", "UCA3 Z",
    "TR1 X", "TR1 Y", "TR1 Z", "TR2 X", "TR2 Y", "TR2 Z",
    "WCN X", "WCN Y", "WCN Z", "SPN X", "SPN Y", "SPN Z",
    "Objective function 0 is best",
    "Camber down (deg)", "Camber up (deg)",
    "Toe down (deg)", "Toe up (deg)", 
    "Caster angle (deg)",
    "Roll centre height (mm)",
    "Caster trail (mm)", "Scrub radius (mm)",
    "Kingpin angle (deg)", 
    "Anti drive feature(%)", "Anti brake feature(%)",
    "Half track change down (mm)",
    "Wheelbase change down (mm)",
    "Half track change up (mm)", "Wheelbase change up (mm)",
    "Method", "Execution time(s)"]

test_initial_hardpoints =[
    -411.709, -132.316, 			# lca1 x y z
	-408.195, -126.205, 					# lca2
	-2135, -600, -140, 								# lca3
	-416.249, -275.203, 					# uca1
	-417.314, -270.739, 					# uca2
	-2153, -578, -315, 								# uca3
	-2234.8, -411.45, -194.6, 						# tr1
	-2225, -582, -220,								# tr2
]

# test_hps gives nan for objective function value
test_hps = [ -406.70241222,  -125.28024281,  -378.86606791,   -93.95976881,
       -2153.23706113,  -620.6565911 ,  -165.90714882,  -442.2011551 ,
        -322.01104566,  -353.46967717,  -267.41468415, -2140.86780456,
        -595.04537695,  -343.49183508, -2231.15803313,  -453.21632971,
        -188.09879557, -2201.74463637,  -554.01927207,  -257.10647785]




#print(f"call obj: {call_suspension_objective(test_hps)}")
#
#start_time=time.time()
# # cobyla
#sol = minimize(call_suspension_objective, random_initial_susp(), constraints=hps_constraints_cobyla, method='COBYLA',options=#{"maxiter":2000,"disp":True})
# # slsqp
##sol = minimize(call_suspension_objective, random_initial_susp(), constraints=hps_constraints_slsqp, bounds=hps_bounds_slsqp, #method='SLSQP', options={"maxiter":200,"disp":True})
#print(sol)
#time.sleep(0.1)
#if sol.success==False:
#    print("nije constrint")
#
#if sol.success==True:
#    print("uspjelo constrint")
#print(f"trajalo je {time.time()-start_time-0.1}")


if __name__ == "__main__":

    started_on = datetime.datetime.now()
    print(started_on)
    start_time = time.time()
    # koliko dugo ce se vrtiti optimizacije
    running_time = 100

    function_expiration_time = 5  # time during which an optimization cycle has to give a result otherwise is terminated
    # jedan proces se iskoristava za managera na sto treba paziti
    num_of_processes = 6

    ## MANAGER objdeinjuje rjesenja pojedinih procesa u jednu listu
    manager = mp.Manager()
    return_dict = manager.list()
    # randomizirani odabir metode optimizacije, sto ce se ubacivati u multiprocessing
    optim_process = [optim_process_cobyla, optim_process_slsqp]
    # randomizer = np.random.choice

    processes = {}
    for i in range(num_of_processes):
        p = mp.Process(target=optim_process_cobyla, args=(random_initial_susp(), return_dict))
        p.start()
        # dodaje pokrenuti proces u dict sa kljucem naziva process 1,2,... te value je lista koja ima vrijeme kad je zapocet
        # proces i sam proces
        processes[f"process {i}"] = [p, time.time()]
    print(processes)
    while time.time() - start_time < running_time:
        # print("while petlja")
        for current_process in processes.keys():
            current_time = time.time()
            # provjerava je li proces izbacio rezultat pomocu .exitcode koji daje None u slucaju da nije i pomocu vremena trajanja procesa
            if processes[current_process][0].exitcode is None and current_time - processes[current_process][1] > function_expiration_time:
                print(f"zaglavio {current_process} PID {processes[current_process][0].pid}, iskoristeno vrijeme {current_time - processes[current_process][1]}")
                # terimnates old process that has elapsed too much time
                processes[current_process][0].terminate()
                processes[current_process][0].join()
                # starts new fresh process

                processes[current_process] = [mp.Process(target=optim_process_cobyla, args=(random_initial_susp(), return_dict)), time.time()]
                processes[current_process][0].start()

            elif processes[current_process][0].exitcode == 0:
                # zapocinje novi proces nakon sto je treuntni dao rjesenje
                processes[current_process][0].join()
                processes[current_process] = [mp.Process(target=optim_process_cobyla, args=(random_initial_susp(), return_dict)), time.time()]
                processes[current_process][0].start()

        time.sleep(0.1)

    for current_process in processes.keys():
        processes[current_process][0].terminate()
        processes[current_process][0].join()
    print(processes)

    ## MANAGER RETURN
    print(f"pronadeno {len(return_dict)} rjesenja")
    pandas_container = pd.DataFrame(list(return_dict), columns=column_names)
    print(pandas_container)

    # if file does not exist write header
    # else it exists so append without writing the header
    if not os.path.isfile('hps_iterations.csv'):
        pandas_container.to_csv('hps_iterations.csv', header=column_names, index=False, sep=";")
    else:
        not_written = True
        while not_written:
            try:
                pandas_container.to_csv('hps_iterations.csv', mode='a', header=False, index=False, sep=";")
                not_written = False
            except PermissionError:

                input("nije zapisano, zatvori csv i pokusaj ponovno: ")

    print("gotov program")
    print(f"started on: {started_on}")
    print(f"ended on: {datetime.datetime.now()}")
    # print(pd.read_csv("filename.csv", sep=";"))
