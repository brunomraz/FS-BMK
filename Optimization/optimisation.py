"""FORMULA STUDENT SUSPENSION BASIC MOTION AND KINEMATICS OPTIMIZATOR"""

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
import sys

S = Suspension




# used as input to minimization function inside optmiziation method
def random_initial_susp():
    hps_bounds_slsqp = [runif(S.lca1y_lo, S.lca1y_up), runif(S.lca1z_lo, S.lca1z_up),
        runif(S.lca2y_lo, S.lca2y_up), runif(S.lca2z_lo, S.lca2z_up),
        runif(S.lca3x_lo, S.lca3x_up), runif(S.lca3y_lo, S.lca3y_up), runif(S.lca3z_lo, S.lca3z_up),
        runif(S.uca1y_lo, S.uca1y_up), runif(S.uca1z_lo, S.uca1z_up),
        runif(S.uca2y_lo, S.uca2y_up), runif(S.uca2z_lo, S.uca2z_up),
        runif(S.uca3x_lo, S.uca3x_up), runif(S.uca3y_lo, S.uca3y_up), runif(S.uca3z_lo, S.uca3z_up),
        runif(S.tr1x_lo, S.tr1x_up), runif(S.tr1y_lo, S.tr1y_up), runif(S.tr1z_lo, S.tr1z_up),
        runif(S.tr2x_lo, S.tr2x_up), runif(S.tr2y_lo, S.tr2y_up), runif(S.tr2z_lo, S.tr2z_up)]

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
        # APPENDA rjesenje jedne iteracije u mp.Manager.list()
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
        # APPENDA rjesenje jedne iteracije u mp.Manager.list()
        #shared_list.append(np.append(call_S_check(sol.x)[3], ("SLSQP",
        #end_time - start_time)))

    else:
        print("nisu constraintovi pogodeni")


# print(optim_process_cobyla(random_initial_susp()[0])[0])
column_names = ["LCA1 X", "LCA1 Y", "LCA1 Z", "LCA2 X", "LCA2 Y", "LCA2 Z", "LCA3 X", "LCA3 Y", "LCA3 Z",
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

test_initial_hardpoints = [-411.709, -132.316, 			# lca1 x y z
	-408.195, -126.205, 					# lca2
	-2135, -600, -140, 								# lca3
	-416.249, -275.203, 					# uca1
	-417.314, -270.739, 					# uca2
	-2153, -578, -315, 								# uca3
	-2234.8, -411.45, -194.6, 						# tr1
	-2225, -582, -220,								# tr2
]

# test_hps gives nan for objective function value
test_hps = [-406.70241222,  -125.28024281,  -378.86606791,   -93.95976881,
       -2153.23706113,  -620.6565911 ,  -165.90714882,  -442.2011551 ,
        -322.01104566,  -353.46967717,  -267.41468415, -2140.86780456,
        -595.04537695,  -343.49183508, -2231.15803313,  -453.21632971,
        -188.09879557, -2201.74463637,  -554.01927207,  -257.10647785]




#print(f"call obj: {call_suspension_objective(test_hps)}")
#
#start_time=time.time()
# # cobyla
#sol = minimize(call_suspension_objective, random_initial_susp(),
#constraints=hps_constraints_cobyla,
#method='COBYLA',options=#{"maxiter":2000,"disp":True})
# # slsqp
##sol = minimize(call_suspension_objective, random_initial_susp(),
##constraints=hps_constraints_slsqp, bounds=hps_bounds_slsqp, #method='SLSQP',
##options={"maxiter":200,"disp":True})
#print(sol)
#time.sleep(0.1)
#if sol.success==False:
#    print("nije constrint")
#
#if sol.success==True:
#    print("uspjelo constrint")
#print(f"trajalo je {time.time()-start_time-0.1}")
if __name__ == "__main__":
    S.lca1x_opt = sys.argv[1]
    S.lca1y_lo = sys.argv[2]
    S.lca1y_up = sys.argv[3]
    S.lca1z_lo = sys.argv[4]
    S.lca1z_up = sys.argv[5]
    S.lca2x_opt = sys.argv[6]
    S.lca2y_lo = sys.argv[7]
    S.lca2y_up = sys.argv[8]
    S.lca2z_lo = sys.argv[9]
    S.lca2z_up = sys.argv[10]
    S.lca3x_lo = sys.argv[11]
    S.lca3x_up = sys.argv[12]
    S.lca3y_lo = sys.argv[13]
    S.lca3y_up = sys.argv[14]
    S.lca3z_lo = sys.argv[15]
    S.lca3z_up = sys.argv[16]
    # UCA
    S.uca1x_opt = sys.argv[17]
    S.uca1y_lo = sys.argv[18]
    S.uca1y_up = sys.argv[19]
    S.uca1z_lo = sys.argv[20]
    S.uca1z_up = sys.argv[21]
    S.uca2x_opt = sys.argv[22]
    S.uca2y_lo = sys.argv[23]
    S.uca2y_up = sys.argv[24]
    S.uca2z_lo = sys.argv[25]
    S.uca2z_up = sys.argv[26]
    S.uca3x_lo = sys.argv[27]
    S.uca3x_up = sys.argv[28]
    S.uca3y_lo = sys.argv[29]
    S.uca3y_up = sys.argv[30]
    S.uca3z_lo = sys.argv[31]
    S.uca3z_up = sys.argv[32]
    # TR1
    S.tr1x_lo = sys.argv[33]
    S.tr1x_up = sys.argv[34]
    S.tr1y_lo = sys.argv[35]
    S.tr1y_up = sys.argv[36]
    S.tr1z_lo = sys.argv[37]
    S.tr1z_up = sys.argv[38]
    # TR2
    S.tr2x_lo = sys.argv[39]
    S.tr2x_up = sys.argv[40]
    S.tr2y_lo = sys.argv[41]
    S.tr2y_up = sys.argv[42]
    S.tr2z_lo = sys.argv[43]
    S.tr2z_up = sys.argv[44]
    # WCN
    S.wcnx_opt = sys.argv[45]
    S.wcny_opt = sys.argv[46]
    S.wcnz_opt = sys.argv[47]
    # SPN
    S.spnx_opt = sys.argv[48]
    S.spny_opt = sys.argv[49]
    S.spnz_opt = sys.argv[50]
    # camber angle
    S.camber_down_pos_lo_lim = sys.argv[51]
    S.camber_down_pos_up_lim = sys.argv[52]
    S.camber_up_pos_lo_lim = sys.argv[53]
    S.camber_up_pos_up_lim = sys.argv[54]
    # toe angle
    S.toe_lopos_lolim = sys.argv[55]
    S.toe_lopos_uplim = sys.argv[56]
    S.toe_uppos_lolim = sys.argv[57]
    S.toe_uppos_uplim = sys.argv[58]
    # caster angle
    S.caster_angle_lolim = sys.argv[59]
    S.caster_angle_uplim = sys.argv[60]
    # roll centre height
    S.roll_centre_height_lolim = sys.argv[61]
    S.roll_centre_height_uplim = sys.argv[62]
    # caster trail
    S.caster_trail_lolim = sys.argv[63]
    S.caster_trail_uplim = sys.argv[64]
    # scrub radius
    S.scrub_radius_lolim = sys.argv[65]
    S.scrub_radius_uplim = sys.argv[66]
    # kingpin angle
    S.kingpin_angle_lolim = sys.argv[67]
    S.kingpin_angle_uplim = sys.argv[68]
    # anti drive
    S.anti_drive_lolim = sys.argv[69]
    S.anti_drive_uplim = sys.argv[70]
    # anti brake
    S.anti_brake_lolim = sys.argv[71]
    S.anti_brake_uplim = sys.argv[72]
    # half track change
    S.half_track_change_down_pos_lolim=sys.argv[73]
    S.half_track_change_down_pos_uplim=sys.argv[74]
    S.half_track_change_up_pos_lolim=sys.argv[75]
    S.half_track_change_up_pos_uplim=sys.argv[76]
    # wheelbase change
    S.wheelbase_change_down_pos_lolim=sys.argv[77]
    S.wheelbase_change_down_pos_uplim=sys.argv[78]
    S.wheelbase_change_up_pos_lolim=sys.argv[79]
    S.wheelbase_change_up_pos_uplim=sys.argv[80]
    # inside wheel free radius
    S.inside_wheel_free_radius_lca3_lolim=sys.argv[81]
    S.inside_wheel_free_radius_lca3_uplim=sys.argv[82]
    S.inside_wheel_free_radius_uca3_lolim=sys.argv[83]
    S.inside_wheel_free_radius_uca3_uplim=sys.argv[84]
    S.inside_wheel_free_radius_tr2_lolim=sys.argv[85]
    S.inside_wheel_free_radius_tr2_uplim=sys.argv[86]
    # distance to wcn along wcn-spn axis
    S.wcn_lca3_distance_lolim=sys.argv[87]
    S.wcn_lca3_distance_uplim=sys.argv[88]
    S.wcn_uca3_distance_lolim=sys.argv[89]
    S.wcn_uca3_distance_uplim=sys.argv[90]
    S.wcn_tr2_distance_lolim=sys.argv[91]
    S.wcn_tr2_distance_uplim=sys.argv[92]
    # general suspension setup
    S.suspension_position=sys.argv[93]
    S.wheel_radius=sys.argv[94]
    S.wheelbase=sys.argv[95]
    S.cog_height=sys.argv[96]
    S.drive_bias=sys.argv[97]
    S.brake_bias=sys.argv[98]
    S.drive_position=sys.argv[99]
    S.brake_position=sys.argv[100]
    S.vertical_movement=sys.argv[101]
    num_of_processes=sys.argv[102]  # sets how many processes will the optimisation use
    running_time=sys.argv[103] # sets how long will the optimisation run



    started_on = datetime.datetime.now()
    print(started_on)
    start_time = time.time()
    
    running_time = 100

    function_expiration_time = 5  # time during which an optimization cycle has to give a result otherwise is terminated

    ## MANAGER objdeinjuje rjesenja pojedinih procesa u jednu listu
    manager = mp.Manager()
    return_dict = manager.list()
    # randomizirani odabir metode optimizacije, sto ce se ubacivati u
    # multiprocessing
    optim_process = [optim_process_cobyla, optim_process_slsqp]
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
