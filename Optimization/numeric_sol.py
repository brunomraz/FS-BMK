import numpy as np
# from numpy.random import uniform as rand
import formulae as f


# from scipy.optimize import minimize
# import timeit
import matplotlib as mpl

mpl.use('Qt4Agg')
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
# import time


# TODO
# napravit decorator funkciju za mjerenje vremena i za plotanje funkcija
# POMOCU while petlje definirati da vrti optimizaciju sve dok nije ispod neke brojke, npr. 0.1 te ispisuje vrijednost objektne funkcije trenutnog najboljeg rjesenja
# suziti podrucje za koje se dobivaju dobre ocjene i na taj nacin uvjetovat pogadanje oba uvjeta funkcije
# napravit decorator funkciju za mjerenje vremena i za plotanje funkcija
# POMOCU while petlje definirati da vrti optimizaciju sve dok nije ispod neke brojke, npr. 0.1 te ispisuje vrijednost objektne funkcije trenutnog najboljeg rjesenja
# suziti podrucje za koje se dobivaju dobre ocjene i na taj nacin uvjetovat pogadanje oba uvjeta funkcije
# vidjeti rade li ogranicenja koja nisu matematickog zapisa vec npr pomocu ifova daju neku brojku
# napraviti evolutionary optimizaciju
# prekinuti optimizaciju ako vrti dulje od nekog vremena


class Suspension():
    """kreira cetvrtinu ovjesa koja je definirana xyz koord sustavom gdje x os gleda prema
    straznjem dijelu auta, sama cetvrtina ovjesa se nalazi na negativnom dijelu y osi te Z os gleda gore.
    prema opisanom slijedi da je uca1x vrijednost manja od uca2x, isto i lca1x i lca2x.
    provjeriti treba li- tr2 mora imati x vrijednost vecu od od wcn x vrijednosti"""
    # preciznost koja se trazi kod newton raphsonove metode
    _precision = 0.0001
    # odreduje koliko ce se kotac pomicati prema gore i prema dolje, za npr zTravel ce ici 30mm gore i 30 mm dolje od referentne pozicije
    _zTravel = 30
    # odreduje koliko u koliko tocaka ce racunati koordinate ovjesa kroz hod gore i dolje,
    # npr za gradient=1 ce u konacnici biti poznate samo 3 tocke, referentna, krajnja gornja i krajnja donja
    _gradient = 20

    ## BASIC CAR INPUTS
    # is it front or rear suspension?
    front_susp = False
    # tire radius in mm
    tire_radius = 230
    # wheelbase in mm
    wheelbase = 1530
    # center of gravity height in mm
    cog_height = 300
    # longitudinal weight distribution, describes how much of mass is on front wheels in 0...1 range
    # 0 meaning there is no mass on front wheels and 1 meaning all mass is on front wheels
    front_weight_distribution = 0.4
    # front brake bias, describes how much force the front wheels take during brake in 0...1 range, 0 meaning
    # that front wheels dont brake and 1 meaning that only front wheels brake
    front_brake_bias = 0.7
    # front drive bias, describes how much force the front wheels take during acceleration in 0...1 range, 0 meaning
    # that front wheels dont contribute to acceleration and 1 meaning that only front wheels contribute to acceleration
    front_drive_bias = 0
    # drive and brake system positions, if it is inboard it has value 0, if it is outboard it has value 1, odreduje po kojoj formuli ce se racunati
    # anti lift/squat/dive/rise jer je razlicito ovisno o tome nalazi li se na na ovjesenoj ili neovjesenoj masi
    outboard_drive = False
    outboard_brake = True

    # INPUT VALUES FOR OPTIMIZATION

    # boundaries
    uca1x_lo, uca1x_up, uca1y_lo, uca1y_up, uca1z_lo, uca1z_up = np.array([546.963, 546.963, -500, -350, 200, 300])
    uca2x_lo, uca2x_up, uca2y_lo, uca2y_up, uca2z_lo, uca2z_up = np.array([747.881, 747.881, -500, -350, 200, 300])
    uca3x_lo, uca3x_up, uca3y_lo, uca3y_up, uca3z_lo, uca3z_up = np.array([620, 680, -620, -560, 270, 330])

    lca1x_lo, lca1x_up, lca1y_lo, lca1y_up, lca1z_lo, lca1z_up = np.array([545.066, 545.066, -500, -350, 50, 150])
    lca2x_lo, lca2x_up, lca2y_lo, lca2y_up, lca2z_lo, lca2z_up = np.array([747.547, 747.547, -500, -350, 50, 150])
    lca3x_lo, lca3x_up, lca3y_lo, lca3y_up, lca3z_lo, lca3z_up = np.array([620, 680, -630, -570, 80, 140])

    tr1x_lo, tr1x_up, tr1y_lo, tr1y_up, tr1z_lo, tr1z_up = np.array([690, 780, -500, -350, 150, 250])
    tr2x_lo, tr2x_up, tr2y_lo, tr2y_up, tr2z_lo, tr2z_up = np.array([690, 780, -620, -560, 170, 230])

    wcnx_lo, wcnx_up, wcny_lo, wcny_up, wcnz_lo, wcnz_up = np.array([650, 650, -620.5, -620.5, 200, 200])
    spnx_lo, spnx_up, spny_lo, spny_up, spnz_lo, spnz_up = np.array([650, 650, -595.5, -595.5, 199.27, 199.27])

    # granice toe anglea u gornjoj i donjoj poziciji kotaca
    toe_uppos_uplim = 0.05
    toe_uppos_lolim = 0
    toe_lopos_uplim = 0
    toe_lopos_lolim = -0.08

    # odreduju za koju vrijednost cambera se dobiju najbolje ocjene
    # zapravo se koristi samo _wantedCamberUp_uplim za gornju poziciju kotaca jer se to optimizira
    _wantedCamberUp_uplim = -3.82  # maximum wanted camber for wheel in top position
    _wantedCamberUp_lolim = -3.72  # minimum wanted camber for wheel in top position
    _wantedCamberDown_uplim = 0.25  # maximum wanted camber for wheel in top position
    _wantedCamberDown_lolim = 0.15  # minimum wanted camber for wheel in top position

    # wanted roll centre height in ref pos
    _roll_centre_height_uplim = 75
    _roll_centre_height_lolim = 65

    # wanted caster trail in ref pos (in mm)
    _caster_trail_uplim = 20
    _caster_trail_lolim = 10

    # wanted caster angle in ref pos (in degrees)
    _caster_angle_uplim = 15
    _caster_angle_lolim = 4

    # wanted kingpin angle in ref pos (in degrees)
    _kingpin_angle_uplim = 8
    _kingpin_angle_lolim = 3

    # wanted scrub radius in ref pos (in mm)
    _scrub_radius_uplim = 8
    _scrub_radius_lolim = -8

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije uca3 sa felgom (mm)
    # lca3,  i tr2
    _inside_wheel_free_radius_uca3_uplim = 100
    _inside_wheel_free_radius_uca3_lolim = 60

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije lca3 sa felgom (mm)
    _inside_wheel_free_radius_lca3_uplim = 100
    _inside_wheel_free_radius_lca3_lolim = 60

    # radijus koji definira slobodno mjesto unutar kotaca, tj sluzi za sprjecavanje kolizije tr2 sa felgom (mm)
    _inside_wheel_free_radius_tr2_uplim = 100
    _inside_wheel_free_radius_tr2_lolim = 60

    # wanted minimum distance between plane defined by wcn and line wcn-spn and uca3 (mm)
    _wcn_uca3_distance_uplim = 25

    # wanted minimum distance between plane defined by wcn and line wcn-spn and lca3 (mm)
    _wcn_lca3_distance_uplim = 15

    # wanted minimum distance between plane defined by wcn and line wcn-spn and tr2 (mm)
    _wcn_tr2_distance_uplim = 25

    # half track change upper position
    _half_track_change_uppos_uplim = 3
    _half_track_change_uppos_lolim = 0

    # half track change lower position
    _half_track_change_downpos_uplim = 0
    _half_track_change_downpos_lolim = -10

    # wheelbase change upper position
    _wheelbase_change_uppos_uplim = 1.5
    _wheelbase_change_uppos_lolim = -1.5

    # wheelbase change lower position
    _wheelbase_change_downpos_uplim = 1.5
    _wheelbase_change_downpos_lolim = -1.5

    # wanted anti lift- front suspension drive in percent
    wanted_anti_lift_uplim = 18
    wanted_anti_lift_lolim = 10
    # wanted anti dive- front suspension brake in percent
    wanted_anti_dive_uplim = 20
    wanted_anti_dive_lolim = 0
    # wanted anti squat- rear suspension drive in percent
    wanted_anti_squat_uplim = 20
    wanted_anti_squat_lolim = 0
    # wanted anti rise- rear suspension brake in percent
    wanted_anti_rise_uplim = 20
    wanted_anti_rise_lolim = 0

    # odreduju koliko je siroko podrucje na kojem se dobivaju dobre ocjene, veca vrijednost znaci siljastiju funkciju
    _peakWidthUp = 100
    _peakWidthDown = 100
    _peakWidthUp_vector = 100000
    _peakWidthDown_vector = 100000
    # odreduje koliki je utjecaj objektne funkcije kod pomaka kotaca gore ili dolje
    _upWeightFactor = 0.5
    _downWeightFactor = 0.5

    # ## OCJENJIVANJE PREKO USPOREDIVANJA S OSI KOTACA - nije koristeno
    # # odreduje za koju vrijednost toe anglea se dobiju najbolje ocjene
    # # pozitivan je toe in, negativan je toe out
    # # ako zelimo toe out onda stavljamo negativne vrijednosti, ako zelimo toe in onda pozitivne
    # # ako zelimo negativni camber onda -1 ako pozitivni onda 1
    # _wantedToeUp = -0.05
    # _wantedToeDown = 0.07
    # # zeljeni jedinicni vektor krajnjeg polozaja kotaca s obzirom na camber i toe
    # wanted_wheel_direction_up = np.array([-np.tan(np.deg2rad(_wantedToeUp)), -1, -np.tan(np.deg2rad(_wantedCamberUp_uplim))])
    # wanted_wheel_direction_up = wanted_wheel_direction_up / np.linalg.norm(wanted_wheel_direction_up)
    # wanted_wheel_direction_down = np.array([-np.tan(np.deg2rad(_wantedToeDown)), -1, -np.tan(np.deg2rad(_wantedCamberDown_uplim))])
    # wanted_wheel_direction_down = wanted_wheel_direction_down / np.linalg.norm(wanted_wheel_direction_down)

    def __init__(self, uca1, uca2, uca3, lca1, lca2, lca3, tr1, tr2, wcn, spn):
        self.uca1 = uca1
        self.uca2 = uca2
        self.uca3 = uca3
        self.lca1 = lca1
        self.lca2 = lca2
        self.lca3 = lca3
        self.tr1 = tr1  # tie rod zasad mora imati x vrijednost vecu od wcn
        self.tr2 = tr2  # tie rod zasad mora imati x vrijednost vecu od wcn
        self.wcn = wcn
        self.spn = spn
        # contact patch u referentnoj poziciji
        self.cp_ref = np.cross(np.cross(spn - wcn, np.array([0, 0, -wcn[2]])), spn - wcn) / np.linalg.norm(
            np.cross(np.cross(spn - wcn, np.array([0, 0, -wcn[2]])), spn - wcn)) * Suspension.tire_radius + wcn
        self.ref_pos_roll_centre_height = f.calc_roll_center_height_ref_pos(uca1, uca2, uca3, lca1, lca2, lca3, self.cp_ref)
        # print(self.ref_pos_roll_centre_height)
        self.caster_trail_ref_pos = f.calc_caster_trail_ref_pos(self.uca3, self.lca3, self.cp_ref)
        self.caster_angle_ref_pos = f.calc_caster_angle_ref_pos(self.uca3, self.lca3)
        self.kingpin_angle_ref_pos = f.calc_kingpin_angle_ref_pos(self.uca3, self.lca3)
        self.scrub_radius_ref_pos = f.calc_scrub_radius_ref_pos(self.uca3, self.lca3, self.cp_ref)

        self.anti_drive_feature, self.anti_brake_feature = f.calc_anti_features_ref_pos(Suspension.cog_height, Suspension.wheelbase,
                                                                                        Suspension.front_brake_bias, Suspension.front_drive_bias,
                                                                                        uca1, uca2, uca3, lca1, lca2, lca3, wcn, self.cp_ref,
                                                                                        front_susp=Suspension.front_susp, outboard_brake=Suspension.outboard_brake,
                                                                                        outboard_drive=Suspension.outboard_drive)


    def __str__(self):
        self.calculateMovement()
        if Suspension.front_susp:
            anti_drive_name = "anti lift"
            anti_brake_name = "anti dive"
        else:
            anti_drive_name = "anti squat"
            anti_brake_name = "anti rise"
        return f"camber up: {self.camberUp[-1]}" + \
               f"\ncamber down: {self.camberDown[-1]}" + \
               f"\ntoe up: {self.toeUp[-1]}" + \
               f"\ntoe down: {self.toeDown[-1]}" + \
               f"\ncaster: {self.caster_trail_ref_pos}" + \
               f"\nrc height: {self.ref_pos_roll_centre_height}" + \
               f"\ncaster trail: {self.caster_trail_ref_pos}" + \
               f"\ncaster angle: {self.caster_angle_ref_pos}" + \
               f"\nkingpin angle: {self.kingpin_angle_ref_pos}" + \
               f"\nscrub radius: {self.scrub_radius_ref_pos}" + \
               f"\nhalf track change upper position: {self.half_track_change_up}" + \
               f"\nhalf track change lower position: {self.half_track_change_down}" + \
               f"\nwheelbase change upper position: {self.wheelbase_change_up}" + \
               f"\nwheelbase change lower position: {self.wheelbase_change_down}" + \
               f"\n{anti_drive_name}: {self.anti_drive_feature}" + \
               f"\n{anti_brake_name}: {self.anti_brake_feature}"

    def calculateMovement(self):
        # LCA derived data, lca_x/y/z_loc su ujedno koordinate vektora potrebnog za ptreborbu iz lca lok kordsus u
        # globalni kord sus
        lca_x_loc,lca_u, lca_v, lca12, lca_len = f.perpen_unit_vectors(self.lca1, self.lca2, self.lca3)
        # UCA derived data
        uca_x_loc,uca_u, uca_v, uca12, uca_len = f.perpen_unit_vectors(self.uca1, self.uca2, self.uca3)
        # upright 1
        up1_len = np.linalg.norm(self.uca3 - self.lca3)  # length from uca3 to lca3 points
        ### CALCULATION ###

        # LCA3 positions
        self.lca_travel_up, self.lca_travel_down = \
            f.spatial_circle(self.lca3, lca12, lca_u, lca_v, lca_len, zTravel=Suspension._zTravel, gradient=Suspension._gradient,
                             x_axis=lca_x_loc,
                             y_axis=lca_u,
                             z_axis=lca_v)

        # UCA3 positions
        uca_theta_up = \
            f.sphere_circle_intersect(uca12, uca_u, uca_v, uca_len, self.lca_travel_up, up1_len, asump=0, precision=Suspension._precision)
        uca_theta_down = \
            f.sphere_circle_intersect(uca12, uca_u, uca_v, uca_len, self.lca_travel_down, up1_len, asump=0, precision=Suspension._precision)
        # ovdje je nebitan gradijent posto uzima gotove input arraye za up i down
        self.uca_travel_up, self.uca_travel_down = \
            f.spatial_circle(self.uca3, uca12, uca_u, uca_v, uca_len, input_arr_up=uca_theta_up, input_arr_down=uca_theta_down)

        # TR2, SPN and WCN positions
        self.tr2_travel_up, self.wcn_travel_up, self.spn_travel_up = \
            f.tr_sphere_circle_intersect(self.lca3, self.uca3, self.tr1, self.tr2, self.lca_travel_up, self.uca_travel_up, self.wcn, self.spn)
        self.tr2_travel_down, self.wcn_travel_down, self.spn_travel_down = \
            f.tr_sphere_circle_intersect(self.lca3, self.uca3, self.tr1, self.tr2, self.lca_travel_down, self.uca_travel_down, self.wcn, self.spn)

        # Contact Patch position
        self.cp_up = f.contact_patch_pos(self.wcn_travel_up, self.spn_travel_up, Suspension.tire_radius)
        self.cp_down = f.contact_patch_pos(self.wcn_travel_down, self.spn_travel_down, Suspension.tire_radius)

        # HALF track change- izbacuje razliku y vrijednosti izmedu krajnje i referentne pozicije contact_patcha, ako je
        # apsolutna vrijednost y manja od referentne pozicije onda je minus half_track_change, a ako je veca
        # onda je plus half_track_change
        self.half_track_change_up = self.cp_ref[1] - self.cp_up[-1, 1]
        self.half_track_change_down = self.cp_ref[1] - self.cp_down[-1, 1]

        # wheelbase change izbacuje razliku x vrijednosti izmedu krajnje i referentne pozicije wcn, ako je
        # se wcn pomaknuo prema naprijed u smjeru gibanja auta u odnosu na wcn_ref onda je wheelbase_change
        # pozitivan, u suprotnom je negativan
        self.wheelbase_change_up = self.cp_ref[0] - self.cp_up[-1, 0]
        self.wheelbase_change_down = self.cp_ref[0] - self.cp_down[-1, 0]

        # CAMBER in wheel travel UP
        self.camberUp = f.camber_angle(self.wcn_travel_up, self.spn_travel_up)
        self.camberDown = f.camber_angle(self.wcn_travel_down, self.spn_travel_down)
        # TOE camber
        self.toeUp = f.toe_angle(self.wcn_travel_up, self.spn_travel_up)
        self.toeDown = f.toe_angle(self.wcn_travel_down, self.spn_travel_down)

        a = 1

    """constraints varijabli za optimizaciju, inputi za constraintove moraju biti usuglaseni s 
    inputima za funkciju koju optimiziramo, tj call_suspension_objective po pitanju indexa varijabli,
    naziv inuta hps je zadrzan samo radi asocijacije/preglednosti. 
    za jednadzbu je pretpostavka da to sto pise znaci da je vece jednako od 0 ili jednako 0 ovisno hoce
    li se kasnije izraz definirati kao nejednakost ili jednakost"""

    # def vectore_score(self):
    #     # racuna ciljnu funkciju preko usporedbe zeljenog vektora i dobivenog vektora, usporedba se radi skalarnim mnozenjem vektora, ako se poklapaju dobiva se 1
    #     # racuna samo za krajnju poziciju kotaca
    #     self.wheel_direction_up = (self.wcn_travel_up[-1] - self.spn_travel_up[-1]) / np.linalg.norm(self.wcn_travel_up[-1] - self.spn_travel_up[-1])
    #     self.vector_dot_prod_up = self.wheel_direction_up.dot(Suspension.wanted_wheel_direction_up)
    #
    #     self.wheel_direction_down = (self.wcn_travel_down[-1] - self.spn_travel_down[-1]) / np.linalg.norm(self.wcn_travel_down[-1] - self.spn_travel_down[-1])
    #     self.vector_dot_prod_down = self.wheel_direction_down.dot(Suspension.wanted_wheel_direction_down)
    #
    #     # racuna ciljnu funkciju preko eksponencijalne i skalarnog umnoska
    #     self.vector_score_up = np.exp(-Suspension._peakWidthUp_vector * np.square(1 - self.vector_dot_prod_up)) * Suspension._upWeightFactor
    #     self.vector_score_down = np.exp(-Suspension._peakWidthDown_vector * np.square(1 - self.vector_dot_prod_down)) * Suspension._downWeightFactor
    #     self.vector_score_sum = 1 - self.vector_score_up - self.vector_score_down

    def camber_score(self):
        # na temelju funkcije za normalnu razdiobu daje ocjenu (0...1) zadovoljstva za dobiveni camber angle
        self.camberUpObjective = np.exp(-Suspension._peakWidthUp * np.square(self.camberUp[-1] - Suspension._wantedCamberUp_uplim)) * Suspension._upWeightFactor
        self.camberDownObjective = np.exp(-Suspension._peakWidthDown * np.square(self.camberDown[-1] - Suspension._wantedCamberDown_uplim)) * Suspension._downWeightFactor
        # print(f"zadovoljstvo cambera je {camberUpObjective}, a sam camber iznosi {self.camberUp[-1]} u gornjoj poziciji kotaca")
        # print(f"zadovoljstvo cambera je {camberDownObjective}, a sam camber iznosi {self.camberDown[-1]} u donjoj poziciji kotaca")
        self.objectiveSum = 1 - self.camberUpObjective - self.camberDownObjective

    def plotPoints(self):
        ### PLOT points
        fig = plt.figure()
        ax = fig.add_subplot(111, projection='3d')

        ax.scatter(self.tr2_travel_up.T[0], self.tr2_travel_up.T[1], self.tr2_travel_up.T[2])  # plots as points tierod2 travel from reference position upwards
        ax.scatter(self.tr2_travel_down.T[0], self.tr2_travel_down.T[1], self.tr2_travel_down.T[2])  # plots as points tierod2 travel from reference position downwards

        ax.scatter(self.lca_travel_up.T[0], self.lca_travel_up.T[1], self.lca_travel_up.T[2])  # plots as points LCA3 travel from reference position upwards
        ax.scatter(self.lca_travel_down.T[0], self.lca_travel_down.T[1], self.lca_travel_down.T[2])  # plots as points LCA3 travel from reference position downwards

        ax.scatter(self.uca_travel_up.T[0], self.uca_travel_up.T[1], self.uca_travel_up.T[2])  # plots as points UCA3 travel from reference position upwards
        ax.scatter(self.uca_travel_down.T[0], self.uca_travel_down.T[1], self.uca_travel_down.T[2])  # plots as points UCA3 travel from reference position downwards

        ax.scatter(self.wcn_travel_up.T[0], self.wcn_travel_up.T[1], self.wcn_travel_up.T[2])  # plots as points WCN travel from reference position upwards
        ax.scatter(self.wcn_travel_down.T[0], self.wcn_travel_down.T[1], self.wcn_travel_down.T[2])  # plots as points WCN travel from reference position downwards

        ax.scatter(self.spn_travel_up.T[0], self.spn_travel_up.T[1], self.spn_travel_up.T[2])  # plots as points Spindle travel from reference position upwards
        ax.scatter(self.spn_travel_down.T[0], self.spn_travel_down.T[1], self.spn_travel_down.T[2])  # plots as points Spindle travel from reference position downwards

        ax.scatter(self.cp_up.T[0], self.cp_up.T[1], self.cp_up.T[2])  # plots as points Spindle travel from reference position upwards
        ax.scatter(self.cp_down.T[0], self.cp_down.T[1], self.cp_down.T[2])  # plots as points Spindle travel from reference position downwards


        # plots as lines LCA, UCA and TR links in reference position
        ax.plot([self.lca1[0], self.lca2[0], self.lca3[0]],
                [self.lca1[1], self.lca2[1], self.lca3[1]],
                [self.lca1[2], self.lca2[2], self.lca3[2]])
        ax.plot([self.uca1[0], self.uca2[0], self.uca3[0]],
                [self.uca1[1], self.uca2[1], self.uca3[1]],
                [self.uca1[2], self.uca2[2], self.uca3[2]])
        ax.plot([self.tr1[0], self.tr2[0]],
                [self.tr1[1], self.tr2[1]],
                [self.tr1[2], self.tr2[2]])
        ax.plot([self.lca2[0]], [self.lca2[1]], [self.lca2[2]], "bo")

        # plots as lines XYZ axes
        # ax.plot((0, 0), (0, 0), (0, 100), '-b', label='z-axis')
        # ax.plot((0, 0), (0, 100), (0, 0), '-g', label='y-axis')
        # ax.plot((0, 100), (0, 0), (0, 0), '-r', label='x-axis')

        # bounding box for equal scaling
        # ax.scatter((0,2000,0,0),(0,0,-2000,0),(0,0,0,2000))

        ax.legend()
        plt.show()
    #


"""objective function"""


def call_suspension_objective(hps):
    """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
    wcn, spn uca1x, uca2x, lca1x, lca2x
    input sadrzi redom slijedece brojeve:
    0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
    10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""

    s = Suspension(uca1=np.array([Suspension.uca1x_up, hps[0], hps[1]]),
                   uca2=np.array([Suspension.uca2x_lo, hps[2], hps[3]]),
                   uca3=np.array([hps[4], hps[5], hps[6]]),
                   lca1=np.array([Suspension.lca1x_up, hps[7], hps[8]]),
                   lca2=np.array([Suspension.lca2x_lo, hps[9], hps[10]]),
                   lca3=np.array([hps[11], hps[12], hps[13]]),
                   tr1=np.array([hps[14], hps[15], hps[16]]),
                   tr2=np.array([hps[17], hps[18], hps[19]]),
                   wcn=np.array([Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo]),
                   spn=np.array([Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up]))
    s.calculateMovement()
    s.camber_score()
    # s.plotPoints()
    # print(f"s objective sum: {s.objectiveSum}")
    return s.objectiveSum


# def call_suspension_objective_vector(hps):
#     """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
#     wcn, spn uca1x, uca2x, lca1x, lca2x
#     input sadrzi redom slijedece brojeve:
#     0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
#     10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""
#
#     s = Suspension(uca1=np.array([Suspension.uca1x_up, hps[0], hps[1]]),
#                    uca2=np.array([Suspension.uca2x_lo, hps[2], hps[3]]),
#                    uca3=np.array([hps[4], hps[5], hps[6]]),
#                    lca1=np.array([Suspension.lca1x_up, hps[7], hps[8]]),
#                    lca2=np.array([Suspension.lca2x_lo, hps[9], hps[10]]),
#                    lca3=np.array([hps[11], hps[12], hps[13]]),
#                    tr1=np.array([hps[14], hps[15], hps[16]]),
#                    tr2=np.array([hps[17], hps[18], hps[19]]),
#                    wcn=np.array([Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo]),
#                    spn=np.array([Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up]))
#     s.calculateMovement()
#     s.vectore_score()
#     # s.plotPoints()
#     # print(f"s objective sum: {s.objectiveSum}")
#     return s.vector_score_sum


"""pomocna funkcija ista kao i ciljna samo sto izbacuje vrijednosti cambera za provjeru"""


def call_suspension_check(hps):
    """funkcija prima kao listu sve varijabilne podatke, a u njoj samoj se zadaju konstante kao
    wcn, spn uca1x, uca2x, lca1x, lca2x
    input sadrzi redom slijedece brojeve:
    0-uca1y, 1-uca1z, 2-uca2y, 3-uca2z, 4-uca3x, 5-uca3y, 6-uca3z, 7-lca1y, 8-lca1z, 9-lca2y,
    10-lca2z, 11-lca3x, 12-lca3y, 13-lca3z, 14-tr1x, 15-tr1y, 16-tr1z, 17-tr2x, 18-tr2y, 19-tr2z"""

    s = Suspension(uca1=np.array([Suspension.uca1x_up, hps[0], hps[1]]),
                   uca2=np.array([Suspension.uca2x_lo, hps[2], hps[3]]),
                   uca3=np.array([hps[4], hps[5], hps[6]]),
                   lca1=np.array([Suspension.lca1x_up, hps[7], hps[8]]),
                   lca2=np.array([Suspension.lca2x_lo, hps[9], hps[10]]),
                   lca3=np.array([hps[11], hps[12], hps[13]]),
                   tr1=np.array([hps[14], hps[15], hps[16]]),
                   tr2=np.array([hps[17], hps[18], hps[19]]),
                   wcn=np.array([Suspension.wcnx_lo, Suspension.wcny_up, Suspension.wcnz_lo]),
                   spn=np.array([Suspension.spnx_up, Suspension.spny_up, Suspension.spnz_up]))
    s.calculateMovement()
    # s.plotPoints()
    return [f"camber up: {s.camberUp[-1].round(3)}° camber down: {s.camberDown[-1].round(3)}°",
            s.camberUp[-1].round(3),
            s.camberDown[-1].round(3),
            np.concatenate([
                s.uca1, s.uca2, s.uca3,
                s.lca1, s.lca2, s.lca3,
                s.tr1, s.tr2,
                s.wcn, s.spn,
                [s.camberUp[-1], s.camberDown[-1],
                 s.toeUp[-1], s.toeDown[-1],
                 s.ref_pos_roll_centre_height,
                 s.caster_trail_ref_pos, s.caster_angle_ref_pos,
                 s.kingpin_angle_ref_pos, s.scrub_radius_ref_pos,
                 s.half_track_change_up, s.half_track_change_down,
                 s.wheelbase_change_up, s.wheelbase_change_down,
                 s.anti_drive_feature, s.anti_brake_feature]]).round(3)

            ]


if __name__ == "__main__":
    # suspension1 = Suspension(uca1=np.array([600, -300, 300]),
    #                          uca2=np.array([700, -300, 300]),
    #                          uca3=np.array([650, -600, 300]),
    #                          lca1=np.array([600, -300, 100]),
    #                          lca2=np.array([700, -300, 100]),
    #                          lca3=np.array([650, -600, 100]),
    #                          tr1=np.array([700, -300, 200]),
    #                          tr2=np.array([700, -600, 200]),
    #                          wcn=np.array([650, -600, 200]),
    #                          spn=np.array([650, -550, 200]))
    # print(suspension1)
    # print()
    # vulpesR rear hardpoints translated
    # suspension2 = Suspension(uca1=np.array([546.963, -416.249, 255.133]),
    #                          uca2=np.array([747.881, -417.314, 250.669]),
    #                          uca3=np.array([659.4, -578, 294.93]),
    #                          lca1=np.array([545.066, -411.709, 112.246]),
    #                          lca2=np.array([747.547, -408.195, 106.135]),
    #                          lca3=np.array([641.4, -600, 119.93]),
    #                          tr1=np.array([741.2, -411.45, 174.53]),
    #                          tr2=np.array([731.4, -582, 199.93]),
    #                          wcn=np.array([650, -620.5, 200]),
    #                          spn=np.array([650, -595.5, 199.27]))
    #
    # print(suspension2)
    # print()
    # print()
    suspension3 = Suspension(uca1=np.array([2040.563, -416.249, 275.203]),
                             uca2=np.array([2241.481, -417.314, 270.739]),
                             uca3=np.array([2153, -578, 315]),
                             lca1=np.array([2038.666, -411.709, 132.316]),
                             lca2=np.array([2241.147, -408.195, 126.205]),
                             lca3=np.array([2135, -600, 140]),
                             tr1=np.array([2234.8, -411.45, 194.6]),
                             tr2=np.array([2225, -582, 220]),
                             wcn=np.array([2143.6, -620.5, 220.07]),
                             spn=np.array([2143.6, -595.5, 219.34]))
    print(suspension3)
    suspension3.camber_score()
    print(suspension3.objectiveSum)

    suspension3.plotPoints()
    input("zavrsio program, pritisni bilokoju tpiku")
