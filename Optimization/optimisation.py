"""FORMULA STUDENT SUSPENSION BASIC MOTION AND KINEMATICS OPTIMIZATOR"""

import sys
sys.path.append(r"C:/dev/FS-BMK/Optimization")#'my/path/to/module/folder') "C:\dev\FS-BMK\Optimization"
import time
import os
import pandas as pd
import ctypes as c
import numpy as np
from scipy.optimize import minimize
from numpy.random import uniform as rand
from numeric_sol import call_suspension_objective, Suspension
import numeric_sol as ns
import multiprocessing as mp
from constraints import hps_constraints_cobyla, hps_constraints_slsqp
import datetime
from random import uniform as runif


S = Suspension

# used as input to minimization function inside optmiziation method
def random_initial_susp():
    hps_bounds_slsqp = [
        runif(S.hps_boundaries[1], S.hps_boundaries[2]), runif(S.hps_boundaries[3], S.hps_boundaries[4]),
        runif(S.hps_boundaries[6], S.hps_boundaries[7]), runif(S.hps_boundaries[8], S.hps_boundaries[9]),
        runif(S.hps_boundaries[10], S.hps_boundaries[11]), runif(S.hps_boundaries[12], S.hps_boundaries[13]), runif(S.hps_boundaries[14], S.hps_boundaries[15]),
        runif(S.hps_boundaries[17], S.hps_boundaries[18]), runif(S.hps_boundaries[19], S.hps_boundaries[20]),
        runif(S.hps_boundaries[22], S.hps_boundaries[23]), runif(S.hps_boundaries[24], S.hps_boundaries[25]),
        runif(S.hps_boundaries[26], S.hps_boundaries[27]), runif(S.hps_boundaries[28], S.hps_boundaries[29]), runif(S.hps_boundaries[30], S.hps_boundaries[31]),
        runif(S.hps_boundaries[32], S.hps_boundaries[33]), runif(S.hps_boundaries[34], S.hps_boundaries[35]), runif(S.hps_boundaries[36], S.hps_boundaries[37]),
        runif(S.hps_boundaries[38], S.hps_boundaries[39]), runif(S.hps_boundaries[40], S.hps_boundaries[41]), runif(S.hps_boundaries[42], S.hps_boundaries[43])]

    return hps_bounds_slsqp


def optim_process_cobyla(initial_hps, shared_list):
    # global hps_constraints
    start_time = time.time()
    sol = minimize(call_suspension_objective, 
                   initial_hps,
                   constraints=hps_constraints_cobyla,
                   method='COBYLA',
                   options={"maxiter": 2000})
    end_time = time.time()

    if sol.success == True:
        print("optimisation iteration done successfully")
        # APPENDS solution of one optimisation to mp.Manager.list()
        shared_list.append([Suspension.obj_func_res_c.value]+Suspension.return_hps_and_parameters() + ["COBYLA", end_time - start_time])

    else:
        print("Constraints not satisfied")


# print(optim_process_cobyla(random_initial_susp()[0])[0])
column_names = ["Objective function 0 is best",
    "LCA1 X", "LCA1 Y", "LCA1 Z", "LCA2 X", "LCA2 Y", "LCA2 Z", "LCA3 X", "LCA3 Y", "LCA3 Z",
    "UCA1 X", "UCA1 Y", "UCA1 Z", "UCA2 X", "UCA2 Y", "UCA2 Z", "UCA3 X", "UCA3 Y", "UCA3 Z",
    "TR1 X", "TR1 Y", "TR1 Z", "TR2 X", "TR2 Y", "TR2 Z",
    "WCN X", "WCN Y", "WCN Z", "SPN X", "SPN Y", "SPN Z",
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
    "LCA3 free radius (mm)", "UCA3 free radius (mm)", "TR2 free radius (mm)",
    "LCA3 WCN distance (mm)", "UCA3 WCN distance (mm)", "TR2 WCN distance (mm)",

    "Method", "Execution time(s)"]


#test_initial_hardpoints = [
#    -411.709, -132.316, # lca1 x y z
#	-408.195, -126.205, # lca2
#	-2135, -600, -140, # lca3
#	-416.249, -275.203, # uca1
#	-417.314, -270.739, # uca2
#	-2153, -578, -315, # uca3
#	-2234.8, -411.45, -194.6, # tr1
#	-2225, -582, -220, # tr2
#]
#
## test_hps gives nan for objective function value
#test_hps = [-406.70241222, -125.28024281, -378.86606791, -93.95976881,
#       -2153.23706113, -620.6565911 , -165.90714882, -442.2011551 ,
#        -322.01104566, -353.46967717, -267.41468415, -2140.86780456,
#        -595.04537695, -343.49183508, -2231.15803313, -453.21632971,
#        -188.09879557, -2201.74463637, -554.01927207, -257.10647785]



 # slsqp
#sol = minimize(call_suspension_objective, random_initial_susp(),
#constraints=hps_constraints_slsqp, bounds=hps_bounds_slsqp, #method='SLSQP',
#options={"maxiter":200,"disp":True})


#print(f"call obj: {call_suspension_objective(test_hps)}")
#
#start_time=time.time()
# # cobyla
#sol = minimize(call_suspension_objective, test_initial_hardpoints , #
#random_initial_susp(),
#                constraints=hps_constraints_cobyla,
#                method='COBYLA',options={"maxiter":2000,"disp":True})
#
#print(sol)
#time.sleep(0.1)
#if sol.success==False:
#    print("nije constrint")
#
#if sol.success==True:
#    print("uspjelo constrint")
#print(f"trajalo je {time.time()-start_time-0.1}")
if __name__ == "__main__":
   
    with open("sysargv.txt", "w") as file1:
        toFile = ""
        for i in range(188):
            toFile+=sys.argv[i] + " "
        # Writing data to a file
        file1.write(toFile)

    for i in range(len(S.hps_boundaries)):
        S.hps_boundaries[i] = float(sys.argv[i + 1])
    
    for i in range(len(S.characteristics_weight_factors)):
        S.characteristics_boundaries[2*i] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 1])
        S.characteristics_boundaries[2*i+1] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 2])
        S.characteristics_target_values_c[i] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 3])
        S.characteristics_weight_factors_c[i] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 4])
        S.peak_width_values_c[i] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 5])
        S.peak_flatness_values_c[i] = float(sys.argv[len(S.hps_boundaries) + 6 * i + 6])
        #print(f"S.chars_bounds {S.characteristics_boundaries[2*i]} sys.argv {float(sys.argv[len(S.hps_boundaries) + 4 * i + 1])}")
        #print(f"S.chars_bounds {S.characteristics_boundaries[2*i+1]} sys.argv {float(sys.argv[len(S.hps_boundaries) + 4 * i + 2])}")
        #print(f"S.charstargets {S.characteristics_target_values_c[i]} sys.argv {float(sys.argv[len(S.hps_boundaries) + 4 * i + 3])}")
        #print(f"S.wfs {S.characteristics_weight_factors_c[i]} sys.argv {float(sys.argv[len(S.hps_boundaries) + 4 * i + 4])}")
        #a=input("wait loop")

    # general suspension setup
    S.suspension_position_c = int(sys.argv[177])
    S.wheel_radius_c = float(sys.argv[178])
    S.wheelbase_c = float(sys.argv[179])
    S.cog_height_c = float(sys.argv[180])
    S.drive_bias_c = float(sys.argv[181])
    S.brake_bias_c = float(sys.argv[182])
    S.drive_position_c = int(sys.argv[183])
    S.brake_position_c = int(sys.argv[184])
    S.vertical_movement_c = float(sys.argv[185])
    num_of_processes = int(sys.argv[186])  # sets how many processes will the optimisation use
    running_time = float(sys.argv[187]) # sets how long will the optimisation run


    



    
    with open("suspParams.txt", "w") as file1:
        # Writing data to a file
        stringParam = ""
        for i in range(len(S.hps_boundaries)):
            stringParam+=str(S.hps_boundaries[i]) + " "
        stringParam+="\n"
        print(f"i value {i}")
        for i in range(len(S.characteristics_target_values_c)):
            stringParam += str(S.characteristics_boundaries[2 * i] ) + " "
            stringParam += str(S.characteristics_boundaries[2 * i + 1]) + " "
            stringParam += str(S.characteristics_target_values_c[i]) + " "
            stringParam += str(S.characteristics_weight_factors_c[i]) + " "
            stringParam += str(S.peak_width_values_c[i]) + " "
            stringParam += str(S.peak_flatness_values_c[i]) + "\n "


        stringParam+=str(S.suspension_position_c) + "\n"
        stringParam+=str(S.wheel_radius_c) + "\n"
        stringParam+=str(S.wheelbase_c) + "\n"
        stringParam+=str(S.cog_height_c) + "\n"
        stringParam+=str(S.drive_bias_c) + "\n"
        stringParam+=str(S.brake_bias_c) + "\n"
        stringParam+=str(S.drive_position_c) + "\n"
        stringParam+=str(S.brake_position_c) + "\n"
        stringParam+=str(S.vertical_movement_c) + "\n"
        stringParam+=sys.argv[186] + "\n"
        stringParam+=sys.argv[187] + " "


        file1.write(stringParam)

    
    if S.drive_bias != 0: # if drive bias is not zero than add anti drive constraints to the rest
        pass  # remove front anti drive related constraints, remove rear anti drive related
              # constraints in any case
    if S.brake_bias == 0:
        pass  # remove front anti brake related constraints, remove rear anti brake related
              # constraints in any case


    #num_of_processes = 8
    #running_time = 50

    started_on = datetime.datetime.now()
    print(started_on)
    start_time = time.time()
    

    function_expiration_time = 5  # time during which an optimization cycle has to give a result otherwise is
                                  # terminated

    ## MANAGER objdeinjuje rjesenja pojedinih procesa u jednu listu
    manager = mp.Manager()
    return_dict = manager.list()
    # randomizirani odabir metode optimizacije, sto ce se ubacivati u
    # multiprocessing
    #optim_process = [optim_process_cobyla, optim_process_slsqp]
    # randomizer = np.random.choice

    processes = {}
    for i in range(num_of_processes):
        p = mp.Process(target=optim_process_cobyla, args=(random_initial_susp(), return_dict))
        p.start()
        # dodaje pokrenuti proces u dict sa kljucem naziva process 1,2,...  te
        # value je lista koja ima vrijeme kad je zapocet
        # proces i sam proces
        processes[f"process {i}"] = [p, time.time()]
    print(processes)
    while time.time() - start_time < running_time:
        # print("while petlja")
        for current_process in processes.keys():
            current_time = time.time()
            # provjerava je li proces izbacio rezultat pomocu .exitcode koji
            # daje None u slucaju da nije i pomocu vremena trajanja procesa
            if processes[current_process][0].exitcode is None and current_time - processes[current_process][1] > function_expiration_time:
                print(f"process stuck: {current_process} PID {processes[current_process][0].pid}, elapsed time {current_time - processes[current_process][1]}")
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
    print(f"found {len(return_dict)} solutions")
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

                input("not written, close csv and try again: ")

    print("program ended successfully")
    print(f"started on: {started_on}")
    print(f"ended on: {datetime.datetime.now()}")
    # print(pd.read_csv("filename.csv", sep=";"))
    