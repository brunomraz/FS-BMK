import numpy as np
from scipy.optimize import minimize
import time
from numpy.random import uniform as rand
from numeric_sol import call_suspension_objective, call_suspension_check, Suspension
import numeric_sol as ns
import pandas as pd
import os
import multiprocessing as mp
from constraints import hps_constraints_cobyla, hps_constraints_slsqp, hps_bounds_slsqp
import datetime

# TODO
# opis multiprocessinga https://stackoverflow.com/questions/26063877/python-multiprocessing-module-join-processes-with-timeout
# normalizirati constraintove
# kompleksna funkcija cilja kako bi se filtrirali rezultati
# ubaciti gotovo rj u optimizaciju kao inicijalno


"""camber u gornjoj poziciji mora biti negativan, dok u donjoj je pozeljan pozitivan,
ali nije nuzno"""

"""multiprocessing modul nemoze koristit lambda funkcije jer ih nemoze picklat"""


# used as input to minimization function inside optmiziation method
def random_initial_susp(row_num=1):
    return np.random.uniform(hps_bounds_slsqp[:, 0], hps_bounds_slsqp[:, 1],
                             size=(row_num, hps_bounds_slsqp.shape[0])).tolist()


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
        shared_list.append(np.append(call_suspension_check(sol.x)[3], ("COBYLA", end_time - start_time)))

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
        shared_list.append(np.append(call_suspension_check(sol.x)[3], ("SQSQP", end_time - start_time)))

    else:
        print("nisu constraintovi pogodeni")


# print(optim_process_cobyla(random_initial_susp()[0])[0])
column_names = ["UCA1 X", "UCA1 Y", "UCA1 Z", "UCA2 X", "UCA2 Y", "UCA2 Z",
                "UCA3 X", "UCA3 Y", "UCA3 Z", "LCA1 X", "LCA1 Y", "LCA1 Z",
                "LCA2 X", "LCA2 Y", "LCA2 Z", "LCA3 X", "LCA3 Y", "LCA3 Z",
                "TR1 X", "TR1 Y", "TR1 Z", "TR2 X", "TR2 Y", "TR2 Z",
                "WCN X", "WCN Y", "WCN Z", "SPN X", "SPN Y", "SPN Z",
                "Camber up (deg)", "Camber down (deg)",
                "Toe up (deg)", "Toe down (deg)",
                "Roll centre height (mm)",
                "Caster trail (mm)", "Caster angle (deg)",
                "Kingpin angle (deg)", "Scrub radius (mm)",
                "Half track change up (mm)", "Half track change down (mm)",
                "Wheelbase change up (mm)", "Wheelbase change down (mm)",
                "Anti drive feature(%)", "Anti brake feature(%)",

                "Method", "Execution time(s)"
                ]

test_initial_hardpoints = [-416.249, 255.133,
                           -417.314, 250.669,
                           659.4, -578, 294.93,
                           -411.709, 112.246,
                           -408.195, 106.135,
                           641.4, -600, 119.93,
                           741.2, -411.45, 174.53,
                           731.4, -582, 199.93]

# uca1 = np.array([546.963, -416.249, 255.133]),
# uca2 = np.array([747.881, -417.314, 250.669]),
# uca3 = np.array([659.4, -578, 294.93]),
# lca1 = np.array([545.066, -411.709, 112.246]),
# lca2 = np.array([747.547, -408.195, 106.135]),
# lca3 = np.array([641.4, -600, 119.93]),
# tr1 = np.array([741.2, -411.45, 174.53]),
# tr2 = np.array([731.4, -582, 199.93]),
# wcn = np.array([650, -620.5, 200]),
# spn = np.array([650, -595.5, 199.27]))

# start_time=time.time()
# # cobyla
# sol = minimize(call_suspension_objective, test_initial_hardpoints, constraints=hps_constraints_cobyla, method='COBYLA',options={"maxiter":2000,"disp":True})
# # slsqp
# # sol = minimize(call_suspension_objective, test_initial_hardpoints, constraints=hps_constraints_slsqp, bounds=hps_bounds_slsqp, method='SLSQP', options={"maxiter": 200,"disp":True})
# print(sol)
# time.sleep(0.1)
# if sol.success==False:
#     print("nije constrint")
#
# if sol.success==True:
#     print("uspjelo constrint")
# print(f"trajalo je {time.time()-start_time-0.1}")


if __name__ == "__main__":
    started_on = datetime.datetime.now()
    print(started_on)
    start_time = time.time()
    # koliko dugo ce se vrtiti optimizacije
    running_time = 100

    function_expiration_time = 80  # time during which an optimization cycle has to give a result otherwise is terminated
    # jedan proces se iskoristava za managera na sto treba paziti
    num_of_processes = 6

    ## MANAGER objdeinjuje rjesenja pojedinih procesa u jednu listu
    manager = mp.Manager()
    return_dict = manager.list()
    # randomizirani odabir metode optimizacije, sto ce se ubacivati u multiprocessing
    optim_process = [optim_process_cobyla, optim_process_slsqp]
    randomizer = np.random.choice

    processes = {}
    for i in range(num_of_processes):
        p = mp.Process(target=randomizer(optim_process), args=(random_initial_susp()[0], return_dict))
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

                processes[current_process] = [mp.Process(target=randomizer(optim_process), args=(random_initial_susp()[0], return_dict)), time.time()]
                processes[current_process][0].start()

            elif processes[current_process][0].exitcode == 0:
                # zapocinje novi proces nakon sto je treuntni dao rjesenje
                processes[current_process][0].join()
                processes[current_process] = [mp.Process(target=randomizer(optim_process), args=(random_initial_susp()[0], return_dict)), time.time()]
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
