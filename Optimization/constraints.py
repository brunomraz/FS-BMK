from numeric_sol import Suspension
import numpy as np


# popis svih mogucih constraintova
# camberx, toex, caster trailx, roll centre zx, half track change, caster
# anglex,
# kingpin offset-direktno vezano uz caster trail, nece se zadavat kao
# ogranicenje,
# scrub radiusx, kingpin anglex, ackermann%, antisquat, antidive, wheelbase
# change,
# ogranicavanje lca3, uca3 i tr2 da budu unutar felge po radijusu x,
# ogranicavanje lca3, uca3 i tr2 da budu unutar felge po udaljenosti od kocnica
# x,
""" constraints must be written in the following form:
e.g.
lowerLimit <= x <= upperLimit
 x - lowerLimit >= 0
-x + upperLimit >= 0 

that is, equation must be in the form greater than zero.
"""

# LCA1 constraint
def cons_lo_lca1y(hps):
    return hps[0] - Suspension.hps_boundaries[1]  
def cons_up_lca1y(hps):
    return -hps[0] + Suspension.hps_boundaries[2] 
def cons_lo_lca1z(hps):
    return hps[1] - Suspension.hps_boundaries[3]  
def cons_up_lca1z(hps):
    return -hps[1] + Suspension.hps_boundaries[4] 

# LCA2 constraint
def cons_lo_lca2y(hps):
    return hps[2] - Suspension.hps_boundaries[6]  
def cons_up_lca2y(hps):
    return -hps[2] + Suspension.hps_boundaries[7] 
def cons_lo_lca2z(hps):
    return hps[3] - Suspension.hps_boundaries[8]  
def cons_up_lca2z(hps):
    return -hps[3] + Suspension.hps_boundaries[9] 

# LCA3 constraint
def cons_lo_lca3x(hps):
    return hps[4] - Suspension.hps_boundaries[10]  
def cons_up_lca3x(hps):
    return -hps[4] + Suspension.hps_boundaries[11] 
def cons_lo_lca3y(hps):
    return hps[5] - Suspension.hps_boundaries[12]  
def cons_up_lca3y(hps):
    return -hps[5] + Suspension.hps_boundaries[13] 
def cons_lo_lca3z(hps):
    return hps[6] - Suspension.hps_boundaries[14]  
def cons_up_lca3z(hps):
    return -hps[6] + Suspension.hps_boundaries[15] 

# UCA1 constraint
def cons_lo_uca1y(hps):
    return hps[7] - Suspension.hps_boundaries[17] 
def cons_up_uca1y(hps):
    return -hps[7] + Suspension.hps_boundaries[18]
def cons_lo_uca1z(hps):
    return hps[8] - Suspension.hps_boundaries[19]
def cons_up_uca1z(hps):
    return -hps[8] + Suspension.hps_boundaries[20]

# UCA2 constraint
def cons_lo_uca2y(hps):
    return hps[9] - Suspension.hps_boundaries[22]  
def cons_up_uca2y(hps):
    return -hps[9] + Suspension.hps_boundaries[23]
def cons_lo_uca2z(hps):
    return hps[10] - Suspension.hps_boundaries[24]
def cons_up_uca2z(hps):
    return -hps[10] + Suspension.hps_boundaries[25]

# UCA3 constraint
def cons_lo_uca3x(hps):
    return hps[11] - Suspension.hps_boundaries[26]
def cons_up_uca3x(hps):
    return -hps[11] + Suspension.hps_boundaries[27]
def cons_lo_uca3y(hps):
    return hps[12] - Suspension.hps_boundaries[28] 
def cons_up_uca3y(hps):
    return -hps[12] + Suspension.hps_boundaries[29]
def cons_lo_uca3z(hps):
    return hps[13] - Suspension.hps_boundaries[30] 
def cons_up_uca3z(hps):
    return -hps[13] + Suspension.hps_boundaries[31]

# TR1 constraint
def cons_lo_tr1x(hps):
    return hps[14] - Suspension.hps_boundaries[32]  
def cons_up_tr1x(hps):
    return -hps[14] + Suspension.hps_boundaries[33] 
def cons_lo_tr1y(hps):
    return hps[15] - Suspension.hps_boundaries[34]  
def cons_up_tr1y(hps):
    return -hps[15] + Suspension.hps_boundaries[35]
def cons_lo_tr1z(hps):
    return hps[16] - Suspension.hps_boundaries[36]  
def cons_up_tr1z(hps):
    return -hps[16] + Suspension.hps_boundaries[37] 

# TR2 constraint
def cons_lo_tr2x(hps):
    return hps[17] - Suspension.hps_boundaries[38] 
def cons_up_tr2x(hps):
    return -hps[17] + Suspension.hps_boundaries[39]
def cons_lo_tr2y(hps):
    return hps[18] - Suspension.hps_boundaries[40] 
def cons_up_tr2y(hps):
    return -hps[18] + Suspension.hps_boundaries[41]
def cons_lo_tr2z(hps):
    return hps[19] - Suspension.hps_boundaries[42] 
def cons_up_tr2z(hps):
    return -hps[19] + Suspension.hps_boundaries[43]


######## SUSPENSION CHARACTERISTICS CONSTRAINTS ############
############################################################
def camber_constraint_down_lolim(hps):
    return Suspension.output_params_optimisation_c[0] - Suspension.characteristics_boundaries[0]


def camber_constraint_down_uplim(hps):
    return -Suspension.output_params_optimisation_c[0] + Suspension.characteristics_boundaries[1]


def camber_constraint_uppos_lolim(hps):
    return Suspension.output_params_optimisation_c[1] - Suspension.characteristics_boundaries[2]


def camber_constraint_uppos_uplim(hps):
    return -Suspension.output_params_optimisation_c[1] + Suspension.characteristics_boundaries[3]


def toe_constraint_down_lolim(hps):
    return Suspension.output_params_optimisation_c[2] - Suspension.characteristics_boundaries[4]


def toe_constraint_down_uplim(hps):
    return -Suspension.output_params_optimisation_c[2] + Suspension.characteristics_boundaries[5]


def toe_constraint_uppos_lolim(hps):
    return Suspension.output_params_optimisation_c[3] - Suspension.characteristics_boundaries[6]


def toe_constraint_uppos_uplim(hps):
    return -Suspension.output_params_optimisation_c[3] + Suspension.characteristics_boundaries[7]


def caster_angle_ref_pos_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[4] - Suspension.characteristics_boundaries[8]


def caster_angle_ref_pos_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[4] + Suspension.characteristics_boundaries[9]


def roll_centre_height_ref_pos_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[5] - Suspension.characteristics_boundaries[10]


def roll_centre_height_ref_pos_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[5] + Suspension.characteristics_boundaries[11]


def caster_trail_ref_pos_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[6] - Suspension.characteristics_boundaries[12]


def caster_trail_ref_pos_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[6] + Suspension.characteristics_boundaries[13]


def scrub_radius_ref_pos_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[7] - Suspension.characteristics_boundaries[14]


def scrub_radius_ref_pos_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[7] + Suspension.characteristics_boundaries[15]


def kingpin_angle_ref_pos_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[8] - Suspension.characteristics_boundaries[16]


def kingpin_angle_ref_pos_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[8] + Suspension.characteristics_boundaries[17]


def calc_anti_drive_lolim(hps):
    return Suspension.output_params_optimisation_c[9] - Suspension.characteristics_boundaries[18]


def calc_anti_drive_uplim(hps):
    return -Suspension.output_params_optimisation_c[9] + Suspension.characteristics_boundaries[19]


def calc_anti_brake_lolim(hps):
    return Suspension.output_params_optimisation_c[10] - Suspension.characteristics_boundaries[20]


def calc_anti_brake_uplim(hps):
    return -Suspension.output_params_optimisation_c[10] + Suspension.characteristics_boundaries[21]


def calc_half_track_change_down_lolim(hps):
    return Suspension.output_params_optimisation_c[11] - Suspension.characteristics_boundaries[22]


def calc_half_track_change_down_uplim(hps):
    return -Suspension.output_params_optimisation_c[11] + Suspension.characteristics_boundaries[23]


def calc_half_track_change_up_lolim(hps):
    return Suspension.output_params_optimisation_c[12] - Suspension.characteristics_boundaries[24]


def calc_half_track_change_up_uplim(hps):
    return -Suspension.output_params_optimisation_c[12] + Suspension.characteristics_boundaries[25]


def calc_wheelbase_change_down_lolim(hps):
    return Suspension.output_params_optimisation_c[13] - Suspension.characteristics_boundaries[26]


def calc_wheelbase_change_down_uplim(hps):
    return -Suspension.output_params_optimisation_c[13] + Suspension.characteristics_boundaries[27]


def calc_wheelbase_change_up_lolim(hps):
    return Suspension.output_params_optimisation_c[14] - Suspension.characteristics_boundaries[28]


def calc_wheelbase_change_up_uplim(hps):
    return -Suspension.output_params_optimisation_c[14] + Suspension.characteristics_boundaries[29]


###############  WHEEL PACKAGING CONSTRAINTS ###############
############################################################
def lca3_in_wheel_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[15] - Suspension.characteristics_boundaries[30]


def lca3_in_wheel_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[15] + Suspension.characteristics_boundaries[31]


def uca3_in_wheel_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[16] - Suspension.characteristics_boundaries[32]


def uca3_in_wheel_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[16] + Suspension.characteristics_boundaries[33]


def tr2_in_wheel_constraint_lolim(hps):
    return Suspension.output_params_optimisation_c[17] - Suspension.characteristics_boundaries[34]


def tr2_in_wheel_constraint_uplim(hps):
    return -Suspension.output_params_optimisation_c[17] + Suspension.characteristics_boundaries[35]


def distance_between_wcn_and_lca3_lolim(hps):
    return Suspension.output_params_optimisation_c[18] - Suspension.characteristics_boundaries[36]


def distance_between_wcn_and_lca3_uplim(hps):
    return -Suspension.output_params_optimisation_c[18] + Suspension.characteristics_boundaries[37]


def distance_between_wcn_and_uca3_lolim(hps):
    return Suspension.output_params_optimisation_c[19] - Suspension.characteristics_boundaries[38]


def distance_between_wcn_and_uca3_uplim(hps):
    return -Suspension.output_params_optimisation_c[19] + Suspension.characteristics_boundaries[39]


def distance_between_wcn_and_tr2_lolim(hps):
    return Suspension.output_params_optimisation_c[20] - Suspension.characteristics_boundaries[40]


def distance_between_wcn_and_tr2_uplim(hps):
    return -Suspension.output_params_optimisation_c[20] + Suspension.characteristics_boundaries[41]
    



##### DERIVED FEATURES CONSTRAINTS #######
##########################################
derived_features_constraints = [## CONSTRAINTOVI POZICIJA HARDPOINTA
    ####### POSITIONAL CONSTRAINTS #########
    # lca1

    # lca2

    # lca3
    {'type': 'ineq', 'fun': lca3_in_wheel_constraint_uplim},  # restricts lca3 inside wheel shell upper bound, zamijena za cons_lo_lca3z
    #{'type': 'ineq', 'fun': lca3_in_wheel_constraint_lolim}, # restricts lca3 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_lca3_uplim},  # zadaje gornju granicu udaljenost izmedu wcn ravnine i lca3, umjesto cons_lo_lca3y
    {'type': 'ineq', 'fun': distance_between_wcn_and_lca3_lolim},  # zadaje donju granicu udaljenost izmedu wcn ravnine i lca3, umjesto cons_lo_lca3y

    # uca1

    # uca2

    # uca3
    {'type': 'ineq', 'fun': uca3_in_wheel_constraint_uplim},  # restricts uca3 inside wheel shell upper bound, zamijena za cons_up_uca3z
    #{'type': 'ineq', 'fun': uca3_in_wheel_constraint_lolim}, # restricts uca3 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_uca3_uplim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i uca3, umjesto cons_lo_uca3y
    {'type': 'ineq', 'fun': distance_between_wcn_and_uca3_lolim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i uca3, umjesto cons_lo_uca3y


    # tr1

    # tr2
    {'type': 'ineq', 'fun': tr2_in_wheel_constraint_uplim},  # restricts tr2 inside wheel shell upper bound, zamijena za cons_up_tr2x
    # {'type': 'ineq', 'fun': tr2_in_wheel_constraint_lolim}, # restricts tr2 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_tr2_uplim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i tr2, umjesto cons_lo_tr2y
    {'type': 'ineq', 'fun': distance_between_wcn_and_tr2_lolim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i tr2, umjesto cons_lo_tr2y

    ####### SUSPENSION CHARACTERISTICS CONSTRAINTS #######
    {'type': 'ineq', 'fun': toe_constraint_uppos_uplim},  # toe upper pos upper bound
    {'type': 'ineq', 'fun': toe_constraint_uppos_lolim},  # toe upper pos lower bound
    {'type': 'ineq', 'fun': toe_constraint_down_uplim},  # toe lower pos upper bound
    {'type': 'ineq', 'fun': toe_constraint_down_lolim},  # toe lower pos lower bound

    {'type': 'ineq', 'fun': roll_centre_height_ref_pos_constraint_uplim},  # roll centre ref pos upper bound
    {'type': 'ineq', 'fun': roll_centre_height_ref_pos_constraint_lolim},  # roll centre ref pos lower bound
    {'type': 'ineq', 'fun': caster_trail_ref_pos_constraint_uplim},  # caster trail ref pos upper bound
    {'type': 'ineq', 'fun': caster_trail_ref_pos_constraint_lolim},  # caster trail ref pos lower bound
    {'type': 'ineq', 'fun': caster_angle_ref_pos_constraint_uplim},  # caster angle ref pos upper bound
    {'type': 'ineq', 'fun': caster_angle_ref_pos_constraint_lolim},  # caster angle ref pos lower bound
    {'type': 'ineq', 'fun': kingpin_angle_ref_pos_constraint_uplim},  # kingpin angle ref pos upper bound
    {'type': 'ineq', 'fun': kingpin_angle_ref_pos_constraint_lolim},  # kingpin angle ref pos lower bound
    {'type': 'ineq', 'fun': scrub_radius_ref_pos_constraint_uplim},  # scrub radius ref pos upper bound
    {'type': 'ineq', 'fun': scrub_radius_ref_pos_constraint_lolim},  # scrub radius ref pos lower bound
    {'type': 'ineq', 'fun': calc_half_track_change_up_uplim},  # half track change up pos upper bound
    {'type': 'ineq', 'fun': calc_half_track_change_up_lolim},  # half track change up pos lower bound
    {'type': 'ineq', 'fun': calc_half_track_change_down_uplim},  # half track change down pos upper bound
    {'type': 'ineq', 'fun': calc_half_track_change_down_lolim},  # half track change down pos lower bound
    {'type': 'ineq', 'fun': calc_wheelbase_change_up_uplim},  # wheelbase change up pos upper bound
    {'type': 'ineq', 'fun': calc_wheelbase_change_up_lolim},  # wheelbase change up pos lower bound
    {'type': 'ineq', 'fun': calc_wheelbase_change_down_uplim},  # wheelbase change down pos upper bound
    {'type': 'ineq', 'fun': calc_wheelbase_change_down_lolim},  # wheelbase change down pos lower bound
    {'type': 'ineq', 'fun': calc_anti_drive_uplim},  # anti drive ref pos upper bound
    {'type': 'ineq', 'fun': calc_anti_drive_lolim},  # anti drive ref pos lower bound
    {'type': 'ineq', 'fun': calc_anti_brake_uplim},  # anti brake ref pos upper bound
    {'type': 'ineq', 'fun': calc_anti_brake_lolim},  # anti brake ref pos lower bound
]


###### PROJECT SPACE CONSTRAINTS ########
#########################################
project_space_constraints = [
    # lca1
    {'type': 'ineq', 'fun': cons_up_lca1y},  # lca1 y upper bound
    {'type': 'ineq', 'fun': cons_lo_lca1y},  # lca1 y lower bound
    {'type': 'ineq', 'fun': cons_up_lca1z},  # lca1 z upper bound
    {'type': 'ineq', 'fun': cons_lo_lca1z},  # lca1 z lower bound #######
    # lca2
    {'type': 'ineq', 'fun': cons_up_lca2y},  # lca2 y upper bound
    {'type': 'ineq', 'fun': cons_lo_lca2y},  # lca2 y lower bound
    {'type': 'ineq', 'fun': cons_up_lca2z},  # lca2 z upper bound
    {'type': 'ineq', 'fun': cons_lo_lca2z},  # lca2 z lower bound ######
    # lca3
    {'type': 'ineq', 'fun': cons_up_lca3x},  # lca3 x upper bound
    {'type': 'ineq', 'fun': cons_lo_lca3x},  # lca3 x lower bound
   # {'type': 'ineq', 'fun': cons_up_lca3y}, # lca3 y upper bound
  #  {'type': 'ineq', 'fun': cons_lo_lca3y}, # lca3 y lower bound
    {'type': 'ineq', 'fun': cons_up_lca3z},  # lca3 z upper bound
 #   {'type': 'ineq', 'fun': cons_lo_lca3z}, # lca3 z lower bound
    # uca1
    {'type': 'ineq', 'fun': cons_up_uca1y},  # uca1 y upper bound
    {'type': 'ineq', 'fun': cons_lo_uca1y},  # uca1 y lower bound
    {'type': 'ineq', 'fun': cons_up_uca1z},  # uca1 z upper bound
    {'type': 'ineq', 'fun': cons_lo_uca1z},  # uca1 z lower bound #######
    # uca2
    {'type': 'ineq', 'fun': cons_up_uca2y},  # uca2 y upper bound
    {'type': 'ineq', 'fun': cons_lo_uca2y},  # uca2 y lower bound
    {'type': 'ineq', 'fun': cons_up_uca2z},  # uca2 z upper bound
    {'type': 'ineq', 'fun': cons_lo_uca2z},  # uca2 z lower bound #######
    # uca3
    {'type': 'ineq', 'fun': cons_up_uca3x},  # uca3 x upper bound
    {'type': 'ineq', 'fun': cons_lo_uca3x},  # uca3 x lower bound
   # {'type': 'ineq', 'fun': cons_up_uca3y}, # uca3 y upper bound
  #  {'type': 'ineq', 'fun': cons_lo_uca3y}, # uca3 y lower bound
    #{'type': 'ineq', 'fun': cons_up_uca3z}, # uca3 z upper bound
    {'type': 'ineq', 'fun': cons_lo_uca3z},  # uca3 z lower bound #######

    #{'type': 'ineq', 'fun': cons_lo_lca3z}, # lca3 z lower bound ######
    # tr1
    {'type': 'ineq', 'fun': cons_up_tr1x},  # tr1 x upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1x},  # tr1 x lower bound
    {'type': 'ineq', 'fun': cons_up_tr1y},  # tr1 y upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1y},  # tr1 y lower bound
    {'type': 'ineq', 'fun': cons_up_tr1z},  # tr1 z upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1z},  # tr1 z lower bound ######
    # tr2
    {'type': 'ineq', 'fun': cons_up_tr2x},  # tr2 x upper bound
    #{'type': 'ineq', 'fun': cons_lo_tr2x}, # tr2 x lower bound
    #{'type': 'ineq', 'fun': cons_up_tr2y}, # tr2 y upper bound
   # {'type': 'ineq', 'fun': cons_lo_tr2y}, # tr2 y lower bound
    {'type': 'ineq', 'fun': cons_up_tr2z},  # tr2 z upper bound
    {'type': 'ineq', 'fun': cons_lo_tr2z},  # tr2 z lower bound
]

hps_constraints_cobyla = derived_features_constraints[:] + project_space_constraints[:]

hps_constraints_slsqp = derived_features_constraints[:]


