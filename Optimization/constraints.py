from numeric_sol import Suspension
import numpy as np
import formulae as f


# popis svih mogucih constraintova
# camberx, toex, caster trailx, roll centre zx, half track change, caster anglex,
# kingpin offset-direktno vezano uz caster trail, nece se zadavat kao ogranicenje,
# scrub radiusx, kingpin anglex, ackermann%, antisquat, antidive, wheelbase change,
# ogranicavanje lca3, uca3 i tr2 da budu unutar felge po radijusu x,
# ogranicavanje lca3, uca3 i tr2 da budu unutar felge po udaljenosti od kocnica x,

""" constraints must be written in the following form:
e.g.
lowerLimit <= x <= upperLimit
-x + upperLimit >= 0 
 x - lowerLimit >= 0

that is, equation must be in the form greater than zero.
"""


# LCA1 constraint

def cons_up_lca1y(hps):
    return -hps[0] + Suspension.lca1y_up  # - 250


def cons_lo_lca1y(hps):
    return hps[0] - Suspension.lca1y_lo  # + 350


def cons_up_lca1z(hps):
    return -hps[1] + Suspension.lca1z_up  # + 150


def cons_lo_lca1z(hps):
    return hps[1] - Suspension.lca1z_lo  # - 50


# LCA2 constraint

def cons_up_lca2y(hps):
    return -hps[2] + Suspension.lca2y_up  # - 250


def cons_lo_lca2y(hps):
    return hps[2] - Suspension.lca2y_lo  # + 350


def cons_up_lca2z(hps):
    return -hps[3] + Suspension.lca2z_up  # + 150


def cons_lo_lca2z(hps):
    return hps[3] - Suspension.lca2z_lo  # - 50


# LCA3 constraint
def cons_up_lca3x(hps):
    return -hps[4] + Suspension.lca3x_up  # + 680


def cons_lo_lca3x(hps):
    return hps[4] - Suspension.lca3x_lo  # - 620


def cons_up_lca3y(hps):
    return -hps[5] + Suspension.lca3y_up  # - 570


def cons_lo_lca3y(hps):
    return hps[5] - Suspension.lca3y_lo  # + 630


def cons_up_lca3z(hps):
    return -hps[6] + Suspension.lca3z_up  # + 130


def cons_lo_lca3z(hps):
    return hps[6] - Suspension.lca3z_lo  # - 70



# UCA1 constraint

def cons_up_uca1y(hps):
    return -hps[7] + Suspension.uca1y_up  # - 250


def cons_lo_uca1y(hps):
    return hps[7] - Suspension.uca1y_lo  # + 350


def cons_up_uca1z(hps):
    return -hps[8] + Suspension.uca1z_up  # + 350


def cons_lo_uca1z(hps):
    return hps[8] - Suspension.uca1z_lo  # - 250


# UCA2 constraint

def cons_up_uca2y(hps):
    return -hps[9] + Suspension.uca2y_up  # - 250


def cons_lo_uca2y(hps):
    return hps[9] - Suspension.uca2y_lo  # + 350


def cons_up_uca2z(hps):
    return -hps[10] + Suspension.uca2z_up  # + 350


def cons_lo_uca2z(hps):
    return hps[10] - Suspension.uca2z_lo  # - 250


# UCA3 constraint

def cons_up_uca3x(hps):
    return -hps[11] + Suspension.uca3x_up  # + 680


def cons_lo_uca3x(hps):
    return hps[11] - Suspension.uca3x_lo  # - 620


def cons_up_uca3y(hps):
    return -hps[12] + Suspension.uca3y_up  # - 570


def cons_lo_uca3y(hps):
    return hps[12] - Suspension.uca3y_lo  # + 630


def cons_up_uca3z(hps):
    return -hps[13] + Suspension.uca3z_up  # + 330


def cons_lo_uca3z(hps):
    return hps[13] - Suspension.uca3z_lo  # - 270




# TR1 constraint
def cons_up_tr1x(hps):
    return -hps[14] + Suspension.tr1x_up  # + 750


def cons_lo_tr1x(hps):
    return hps[14] - Suspension.tr1x_lo  # - 655


def cons_up_tr1y(hps):
    return -hps[15] + Suspension.tr1y_up  # - 250


def cons_lo_tr1y(hps):
    return hps[15] - Suspension.tr1y_lo  # + 350


def cons_up_tr1z(hps):
    return -hps[16] + Suspension.tr1z_up  # + 250


def cons_lo_tr1z(hps):
    return hps[16] - Suspension.tr1z_lo  # - 150


# TR2 constraint
def cons_up_tr2x(hps):
    return -hps[17] + Suspension.tr2x_up  # + 730


def cons_lo_tr2x(hps):
    return hps[17] - Suspension.tr2x_lo  # - 670


def cons_up_tr2y(hps):
    return -hps[18] + Suspension.tr2y_up  # - 570


def cons_lo_tr2y(hps):
    return hps[18] - Suspension.tr2y_lo  # + 630


def cons_up_tr2z(hps):
    return -hps[19] + Suspension.tr2z_up  # + 230


def cons_lo_tr2z(hps):
    return hps[19] - Suspension.tr2z_lo  # - 170


def toe_constraint_down_lolim(hps):
    return Suspension.outputParams_c[3] - Suspension.toe_lopos_lolim


def toe_constraint_down_uplim(hps):
    return -Suspension.outputParams_c[3] + Suspension.toe_lopos_uplim


def toe_constraint_uppos_lolim(hps):
    return Suspension.outputParams_c[4] - Suspension.toe_uppos_lolim


def toe_constraint_uppos_uplim(hps):
    return -Suspension.outputParams_c[4] + Suspension.toe_uppos_uplim


def caster_angle_ref_pos_constraint_lolim(hps):
    return Suspension.outputParams_c[5] - Suspension._caster_angle_lolim


def caster_angle_ref_pos_constraint_uplim(hps):
    return -Suspension.outputParams_c[5] + Suspension._caster_angle_uplim


def roll_centre_height_ref_pos_constraint_lolim(hps):
    return Suspension.outputParams_c[6] - Suspension._roll_centre_height_lolim


def roll_centre_height_ref_pos_constraint_uplim(hps):
    return -Suspension.outputParams_c[6] + Suspension._roll_centre_height_uplim


def caster_trail_ref_pos_constraint_lolim(hps):
    return Suspension.outputParams_c[7] - Suspension._caster_trail_lolim


def caster_trail_ref_pos_constraint_uplim(hps):
    return -Suspension.outputParams_c[7] + Suspension._caster_trail_uplim


def scrub_radius_ref_pos_constraint_lolim(hps):
    return Suspension.outputParams_c[8] - Suspension._scrub_radius_lolim


def scrub_radius_ref_pos_constraint_uplim(hps):
    return -Suspension.outputParams_c[8] + Suspension._scrub_radius_uplim


def kingpin_angle_ref_pos_constraint_lolim(hps):
    return Suspension.outputParams_c[9] - Suspension._kingpin_angle_lolim


def kingpin_angle_ref_pos_constraint_uplim(hps):
    return -Suspension.outputParams_c[9] + Suspension._kingpin_angle_uplim


def calc_anti_drive_lolim(hps):
    """odreduje donju vrijednost anti lift znacajke karakteristicnu za drive front susp"""
    return Suspension.outputParams_c[10] - Suspension.wanted_anti_drive_lolim


def calc_anti_drive_uplim(hps):
    """odreduje gornju vrijednost anti lift znacajke karakteristicnu za drive front susp"""
    return -Suspension.outputParams_c[10] + Suspension.wanted_anti_drive_uplim


def calc_anti_brake_lolim(hps):
    """odreduje donju vrijednost anti dive znacajke karakteristicnu za brake front susp"""
    return Suspension.outputParams_c[11] - Suspension.wanted_anti_brake_lolim


def calc_anti_brake_uplim(hps):
    """odreduje gornju vrijednost anti dive znacajke karakteristicnu za brake front susp"""
    return -Suspension.outputParams_c[11] + Suspension.wanted_anti_brake_uplim


def calc_half_track_change_down_lolim(hps):
    return Suspension.outputParams_c[12] - Suspension._half_track_change_downpos_lolim


def calc_half_track_change_down_uplim(hps):
    return -Suspension.outputParams_c[12] + Suspension._half_track_change_downpos_uplim


def calc_wheelbase_change_down_lolim(hps):
    return Suspension.outputParams_c[13] - Suspension._wheelbase_change_downpos_lolim


def calc_wheelbase_change_down_uplim(hps):
    return -Suspension.outputParams_c[13] + Suspension._wheelbase_change_downpos_uplim


def calc_half_track_change_up_lolim(hps):
    return Suspension.outputParams_c[14] - Suspension._half_track_change_uppos_lolim


def calc_half_track_change_up_uplim(hps):
    return -Suspension.outputParams_c[14] + Suspension._half_track_change_uppos_uplim


def calc_wheelbase_change_up_lolim(hps):
    return Suspension.outputParams_c[15] - Suspension._wheelbase_change_uppos_lolim


def calc_wheelbase_change_up_uplim(hps):
    return -Suspension.outputParams_c[15]+ Suspension._wheelbase_change_uppos_uplim


def lca3_in_wheel_constraint_lolim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu"""
    return Suspension.outputParams_c[16] - Suspension._inside_wheel_free_radius_lca3_lolim


def lca3_in_wheel_constraint_uplim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu
    zamijena za ogranicenje po z_lolim"""
    return -Suspension.outputParams_c[16] + Suspension._inside_wheel_free_radius_lca3_uplim


def uca3_in_wheel_constraint_lolim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu"""
    return Suspension.outputParams_c[17] - Suspension._inside_wheel_free_radius_uca3_lolim


def uca3_in_wheel_constraint_uplim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu
    zamijena za ogranicenje po z_uplim"""
    return -Suspension.outputParams_c[17] + Suspension._inside_wheel_free_radius_uca3_uplim


def tr2_in_wheel_constraint_lolim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu"""
    return Suspension.outputParams_c[18] - Suspension._inside_wheel_free_radius_tr2_lolim


def tr2_in_wheel_constraint_uplim(hps):
    """zadaje uvjet unutar kojeg cilindra se mora nalaziti uca3 tocka kako bi bili sigurni da ne
    probada felgu
    zamijena za ogranicenje po x_uplim ako je iza wcn
    zamijena za ogranicenje po x_lolim ako je ispred wcn"""
    return -Suspension.outputParams_c[18] + Suspension._inside_wheel_free_radius_tr2_uplim


def distance_between_wcn_and_lca3_lolim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu lca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_lca3y ogranicenja
    ako je lca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3
    """
    return Suspension.outputParams_c[19] - Suspension._wcn_lca3_distance_lolim


def distance_between_wcn_and_lca3_uplim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu lca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_lca3y ogranicenja
    ako je lca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3
    """
    return -Suspension.outputParams_c[19] + Suspension._wcn_lca3_distance_uplim


def distance_between_wcn_and_uca3_lolim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu uca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_uca3y ogranicenja
    ako je uca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3
    """
    return Suspension.outputParams_c[20] - Suspension._wcn_uca3_distance_lolim


def distance_between_wcn_and_uca3_uplim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu uca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_uca3y ogranicenja
    ako je uca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3
    """
    return -Suspension.outputParams_c[20] + Suspension._wcn_uca3_distance_uplim


def distance_between_wcn_and_tr2_lolim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu lca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_tr2y ogranicenja
    ako je lca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3

    """
    return Suspension.outputParams_c[21] - Suspension._wcn_tr2_distance_lolim


def distance_between_wcn_and_tr2_uplim(hps):
    """ogranicenje za minimalnu udaljenost koja mora biti izmedu lca3 i ravnine koju
    definira tocka wcn i pravac wcn,spn
    moze sluziti umjesto cons_lo_tr2y ogranicenja
    ako je lca3 s vanjske strane ravnine, tj ima negativniju y vrijednost od wcn tada ova funkcija
    vraca negativnu vrijednost razmaka izmedu ravnine i uca3

    """
    return -Suspension.outputParams_c[21] + Suspension._wcn_tr2_distance_uplim
    




# za metode: SLSQP, COBYLA
slsqp_cobyla_common_constraints = [
    ## CONSTRAINTOVI POZICIJA HARDPOINTA
    # lca1

    # lca2

    # lca3
    {'type': 'ineq', 'fun': lca3_in_wheel_constraint_uplim},  # restricts lca3 inside wheel shell upper bound, zamijena za cons_lo_lca3z
    #{'type': 'ineq', 'fun': lca3_in_wheel_constraint_lolim},  # restricts lca3 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_lca3_uplim},  # zadaje gornju granicu  udaljenost izmedu wcn ravnine i lca3, umjesto cons_lo_lca3y
    {'type': 'ineq', 'fun': distance_between_wcn_and_lca3_lolim},  # zadaje donju granicu  udaljenost izmedu wcn ravnine i lca3, umjesto cons_lo_lca3y

    # uca1

    # uca2

    # uca3
    {'type': 'ineq', 'fun': uca3_in_wheel_constraint_uplim},  # restricts uca3 inside wheel shell upper bound, zamijena za cons_up_uca3z
    #{'type': 'ineq', 'fun': uca3_in_wheel_constraint_lolim},  # restricts uca3 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_uca3_uplim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i uca3, umjesto cons_lo_uca3y
    {'type': 'ineq', 'fun': distance_between_wcn_and_uca3_lolim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i uca3, umjesto cons_lo_uca3y


    # tr1

    # tr2
    {'type': 'ineq', 'fun': tr2_in_wheel_constraint_uplim},  # restricts tr2 inside wheel shell upper bound, zamijena za cons_up_tr2x
    # {'type': 'ineq', 'fun': tr2_in_wheel_constraint_lolim},  # restricts tr2 inside wheel shell lower bound
    {'type': 'ineq', 'fun': distance_between_wcn_and_tr2_uplim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i tr2, umjesto cons_lo_tr2y
    {'type': 'ineq', 'fun': distance_between_wcn_and_tr2_lolim},  # zadaje minimalnu potrebnu udaljenost izmedu wcn ravnine i tr2, umjesto cons_lo_tr2y

    ## CONSTRAINTOVI IZVEDENIH ZNACAJKI
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

# za metode: COBYLA
hps_coordinate_components_constraints = [
    ## CONSTRAINTOVI POZICIJA HARDPOINTA
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
   # {'type': 'ineq', 'fun': cons_up_lca3y},  # lca3 y upper bound
  #  {'type': 'ineq', 'fun': cons_lo_lca3y},  # lca3 y lower bound
    {'type': 'ineq', 'fun': cons_up_lca3z},  # lca3 z upper bound
 #   {'type': 'ineq', 'fun': cons_lo_lca3z},  # lca3 z lower bound
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
   # {'type': 'ineq', 'fun': cons_up_uca3y},  # uca3 y upper bound
  #  {'type': 'ineq', 'fun': cons_lo_uca3y},  # uca3 y lower bound
    #{'type': 'ineq', 'fun': cons_up_uca3z},  # uca3 z upper bound
    {'type': 'ineq', 'fun': cons_lo_uca3z},  # uca3 z lower bound #######

    #{'type': 'ineq', 'fun': cons_lo_lca3z},  # lca3 z lower bound ######
    # tr1
    {'type': 'ineq', 'fun': cons_up_tr1x},  # tr1 x upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1x},  # tr1 x lower bound
    {'type': 'ineq', 'fun': cons_up_tr1y},  # tr1 y upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1y},  # tr1 y lower bound
    {'type': 'ineq', 'fun': cons_up_tr1z},  # tr1 z upper bound
    {'type': 'ineq', 'fun': cons_lo_tr1z},  # tr1 z lower bound ######
    # tr2
    {'type': 'ineq', 'fun': cons_up_tr2x},  # tr2 x upper bound
    #{'type': 'ineq', 'fun': cons_lo_tr2x},  # tr2 x lower bound
    #{'type': 'ineq', 'fun': cons_up_tr2y},  # tr2 y upper bound
   # {'type': 'ineq', 'fun': cons_lo_tr2y},  # tr2 y lower bound
    {'type': 'ineq', 'fun': cons_up_tr2z},  # tr2 z upper bound
    {'type': 'ineq', 'fun': cons_lo_tr2z},  # tr2 z lower bound
]

hps_constraints_cobyla = slsqp_cobyla_common_constraints[:] + hps_coordinate_components_constraints[:]

hps_constraints_slsqp = slsqp_cobyla_common_constraints[:]


