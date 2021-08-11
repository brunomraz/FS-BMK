import numpy as np
# from scipy.spatial.transform import Rotation as R
# import time
import numba as nb

"""KORISTI SE COLUMN MAJOR mmnozenje matrica"""


# TODO
# formulae.py:81: RuntimeWarning: invalid value encountered in sqrt
#   theta_upmost_position = -2 * np.arctan((np.sqrt(a[2] ** 2 * r ** 2 + b[2] ** 2 * r ** 2 - c[2] ** 2 + 2 * c[2] * z_coord_up - z_coord_up ** 2) + b[2] * r) / (-a[2] * r + c[2] - z_coord_up))
#
# formulae.py:84: RuntimeWarning: invalid value encountered in sqrt
#  (np.sqrt(a[2] ** 2 * r ** 2 + b[2] ** 2 * r ** 2 - c[2] ** 2 + 2 * c[2] * z_coord_down - z_coord_down ** 2) + b[2] * r) / (-a[2] * r + c[2] - z_coord_down))


@nb.njit(cache=True)
def project_point2line(p1, p2, p3):
    """
    tocka p3 je projicirana na liniju koju cine tocke p1 i p2
    input su vektori zapisani u np.array
    formule za project_point2line slijede iz sympy koda:
    from sympy import Point, Line, symbols
    a, b, c, d, e, f, g, h, i = symbols("a b c d e f g h i")
    p1 = Point(lca_1)
    p2 = Point(lca_2)
    p3 = Point(lca_3)
    p12 = Line(p1, p2).projection(p3)
    p12_np = np.array(p12)
    """
    a, b, c = p1
    d, e, f = p2
    g, h, i = p3
    p12 = np.array([
        (a * ((a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2)
         - (a - d) * ((a - d) * (a - g) + (b - e) * (b - h) + (c - f) *
                      (c - i))) / ((a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2),
        (b * ((a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2) - (
                b - e) * ((a - d) * (a - g) + (b - e) * (b - h) + (
                c - f) * (c - i))) / (
                (a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2),
        (c * ((a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2) - (
                c - f) * ((a - d) * (a - g) + (b - e) * (b - h) + (
                c - f) * (c - i))) / (
                (a - d) ** 2 + (b - e) ** 2 + (c - f) ** 2)
    ])
    return p12


@nb.njit(cache=True)
def perpendicular_point(p1, p2, p3, p4=np.array([0, 0, 0]), c=1):
    """na temelju tri tocke odreduje point5 koja ce sa point4 ciniti pravac
    okomit na ravninu koju cine point1, point2 i point3
    tocka se odreduje pomocu jednadzbe pravca u prostoru za sto sluzi
    konstanta c
    ravnina i smjer pravca se odreduju vektorskim produktom (p1-p3) i
    (p2-p3), a tocka p4 odreduje gdje se nalazi pravac na toj ravnini
    ako nije upisan p4 onda ulogu te tocke preuzima p3"""
    if not p4.all():
        vec13 = p1 - p3
        vec23 = p2 - p3
        plane_normal = np.cross(vec13, vec23)
        p5 = plane_normal * c + p3
    else:
        vec13 = p1 - p3
        vec23 = p2 - p3
        plane_normal = np.cross(vec13, vec23)
        p5 = plane_normal * c + p4
    return p5


# @nb.jit
# @nb.njit(cache=True)
def spatial_circle(startPoint, c, a, b, r, zTravel=0, gradient=30, input_arr_up=None, input_arr_down=None, x_axis=None, y_axis=None, z_axis=None):
    """z input predstavlja pomak po z osi od točke sa najnegativnijom Y vrijednoscu, to se koristi za LCA jer je pretpostavka da pomicanje LCA
    uzrokuje pomicanje svih ostalih komponenti
    input_arr_up i down se koriste ako znamo kuteve za koje hocemo izracunati pomak kotaca, to se koristi za uca nakon sto se izracunaju kutevi zakreta UCA
    c- srediste kruznice
    a- prvi okomiti jedinicni vektor
    b- drugi okomiti jedinicni vektor
    r- radijus kruznice
    gradient- rezolucija, tj koliko tocaka ce izvaditi za odredeni zTravel
    startPoint- tocka od koje pocinje racunati kruzne lukove, tj vanjska tocka ramena
    x/y/z_axis sluze za transformaciju iz lok ks (najvjerojatnije ramena) u glob loksus auta
    treba staviti da je referentna z koordinata vanjska tocka ramena, sada je referentna koord fiktivna unutarnja tocka ramena sto ne valja"""
    # postoji razlika u hodu kotaca ako zadajem hod lca u lokalnom ili globalnom koord sus
    # visinu u lks nade pomocu sjecista kruznice i ravnine u 3d prostoru
    if zTravel != 0:
        """formula for theta follows from inputing 'solve z=c+r*a*cos(theta)+r*b*sin(theta) for theta' to wolfram alpha
        and that formula represents Z position of the parametric spatial circle"""
        """izrada pomaka u lokalnom koord sus kako bi se izbjegle arctg funkcije koje nekad izbacuju lose rezz"""
        rot_matrix_loc_to_glob = np.array([x_axis, y_axis, z_axis])
        rot_matrix_glob_to_loc = rot_matrix_loc_to_glob.T
        line_start_pt_up_global = startPoint - c + (0, 0, zTravel)
        line_start_pt_down_global = startPoint - c - (0, 0, zTravel)
        # plane starting point
        x0_up, y0_up, z0_up = np.dot(line_start_pt_up_global, rot_matrix_glob_to_loc)
        x0_down, y0_down, z0_down = np.dot(line_start_pt_down_global, rot_matrix_glob_to_loc)
        plane_normal_glob = (0, 0, zTravel)
        # plane normal local parameters a,b,c upper pos
        a_norm, b_norm, c_norm = np.dot(plane_normal_glob, rot_matrix_glob_to_loc)
        d_norm_up = -(a_norm * x0_up + b_norm * y0_up + c_norm * z0_up)
        d_norm_down = -(a_norm * x0_down + b_norm * y0_down + c_norm * z0_down)
        ys_up = (np.sqrt(c_norm ** 2 * (r ** 2 * (b_norm ** 2 + c_norm ** 2) - d_norm_up ** 2)) - b_norm * d_norm_up) / (b_norm ** 2 + c_norm ** 2)
        ys_down = (np.sqrt(c_norm ** 2 * (r ** 2 * (b_norm ** 2 + c_norm ** 2) - d_norm_down ** 2)) - b_norm * d_norm_down) / (b_norm ** 2 + c_norm ** 2)
        zs_up = (-b_norm * ys_up - d_norm_up) / c_norm
        zs_down = (-b_norm * ys_down - d_norm_down) / c_norm

        # z_coord_up = startPoint[2] + zTravel
        # theta_upmost_position = -2 * np.arctan((np.sqrt(a[2] ** 2 * r ** 2 + b[2] ** 2 * r ** 2 - c[2] ** 2 + 2 * c[2] * z_coord_up - z_coord_up ** 2) + b[2] * r) / (-a[2] * r + c[2] - z_coord_up))
        # z_coord_down = startPoint[2] - zTravel
        # theta_downmost_position = -2 * np.arctan(
        #     (np.sqrt(a[2] ** 2 * r ** 2 + b[2] ** 2 * r ** 2 - c[2] ** 2 + 2 * c[2] * z_coord_down - z_coord_down ** 2) + b[2] * r) / (-a[2] * r + c[2] - z_coord_down))
        #
        # theta_up = np.linspace(theta_upmost_position / gradient, theta_upmost_position, gradient)
        # theta_down = np.linspace(theta_downmost_position / gradient, theta_downmost_position, gradient)

        z_gradient_up = np.linspace(zs_up / gradient, zs_up, gradient).reshape((gradient, 1))
        z_gradient_down = np.linspace(zs_down / gradient, zs_down, gradient).reshape((gradient, 1))
        # zbog koord sust iz perpen_unit_vectors metode je gore negativno, a dolje pozitivno po z
        out_pt_local_up = (0, 1, 0) * np.sqrt(r ** 2 - z_gradient_up ** 2) + (0, 0, 1) * z_gradient_up  # out_pt_local_down * (0, 0, -1)
        out_pt_local_down = (0, 1, 0) * np.sqrt(r ** 2 - z_gradient_down ** 2) + (0, 0, 1) * z_gradient_down
        # skup vektora mnozi s jedno te istom matricom transformacije i offseta u pravi koord sus
        out_pt_glob_up = np.tensordot(out_pt_local_up, rot_matrix_loc_to_glob, axes=1) + c
        out_pt_glob_down = np.tensordot(out_pt_local_down, rot_matrix_loc_to_glob, axes=1) + c
        return out_pt_glob_up, out_pt_glob_down
    else:
        theta_up = input_arr_up
        theta_down = input_arr_down
        out_pt_glob_up = 1

        """parametric spatial circle formula"""

        travel_up2 = c + r * np.cos(theta_up).reshape(theta_up.shape[0], 1) * a + r * np.sin(theta_up).reshape(theta_up.shape[0], 1) * b
        travel_down2 = c + r * np.cos(theta_down).reshape(theta_down.shape[0], 1) * a + r * np.sin(theta_down).reshape(theta_down.shape[0], 1) * b

        return travel_up2, travel_down2


# @nb.jit
def sphere_circle_intersect(circ_centre, a_vect, b_vect, r_circ, sph_centre, r_sph, asump, precision=0.0001):  # precision entered in percent
    prec = precision

    cx, cy, cz = circ_centre
    ax, ay, az = a_vect
    bx, by, bz = b_vect
    Rc = r_circ
    Rs = r_sph
    n0 = asump

    theta_storage = np.zeros(np.shape(sph_centre)[0])
    """func is function that follows from searching for common point between implicit equation of a sphere and explicit/parametric 
    equation of a spatial circle, by inputing x,y and z from spatial circle to implicit equation of a sphere.
    func_deriv is derivated func later used for finding theta by newton raphson method"""
    func = lambda x, y, z, t: (cx + Rc * ax * np.cos(t) + Rc * bx * np.sin(t) - x) ** 2 + (cy + Rc * ay * np.cos(t) + Rc * by * np.sin(t) - y) ** 2 + (
            cz + Rc * az * np.cos(t) + Rc * bz * np.sin(t) - z) ** 2 - Rs ** 2
    func_deriv = lambda x, y, z, t: 2 * Rc * (bx * np.cos(t) - ax * np.sin(t)) * (cx + Rc * ax * np.cos(t) + Rc * bx * np.sin(t) - x) + 2 * Rc * (by * np.cos(t) - ay * np.sin(t)) * (
            cy + Rc * ay * np.cos(t) + Rc * by * np.sin(t) - y) + 2 * Rc * (bz * np.cos(t) - az * np.sin(t)) * (cz + Rc * az * np.cos(t) + Rc * bz * np.sin(t) - z)

    """iterating over all sphere centres"""
    index = 0

    for sphere_pos in sph_centre:

        xf, yf, zf = sphere_pos
        n1 = n0 - func(xf, yf, zf, n0) / func_deriv(xf, yf, zf, n0)

        """finding theta using newton raphson method, when it finds one/converges it adds it to theta_storage"""
        while np.abs((n1 - n0) / n1) > prec:
            n0 = n1
            n1 = n0 - func(xf, yf, zf, n0) / func_deriv(xf, yf, zf, n0)

        theta_storage[index] = n1
        index += 1
    return theta_storage


# @nb.jit
def tr_sphere_circle_intersect(lca3_init, uca3_init, tr1_init, tr2_init, lca3_travel, uca3_travel, wcn_init, spn_init, asumption=0, precision=0.0001):
    """trazi tocku dodira vanjske tocke spone, tj sfere koju definira vanjska tocka spone i kruznice koju definira vanjska tocka spone na uprightu
    rotacijom te tocke na uprightu oko osi koju definiraju lca3 i uca3 za sve vertikalne pozicije kotaca"""
    prec = precision
    n0 = asumption
    # koordinate centra kruznice
    x, y, z = tr1_init
    # polumjer sfere
    Rs = np.linalg.norm(tr1_init - tr2_init)
    # pocetna pozicija centra kruznice na uprightu
    tr12 = project_point2line(lca3_init, uca3_init, tr2_init)
    # ova vrijednost odreduje gdje se na liniji koja povezuje lca3 i uca3 nalazi srediste kruznice od tr2 na uprightu
    # ta se vrijednost kasnije koristi za odredivanje pozicije kruznice, tj sredista kruznice pri hodu kotaca
    t_param_dist = np.linalg.norm(tr12 - lca3_init) / np.linalg.norm(lca3_init - uca3_init)
    # polumjer kruznice
    Rc = np.linalg.norm(tr12 - tr2_init)
    # pocetna vrijednost pozicije tr2, zapravo je ista kao i sama tocka tr2
    tr2_pos = tr2_init

    # dobivanje lokalnih koordinata wcn i spn u lokalnom koord sus uprighta
    # x axis od pocetne pozicije
    x_axis_init = np.array(tr2_init - project_point2line(lca3_init, uca3_init, tr2_init))
    x_axis_init /= np.linalg.norm(x_axis_init)
    # z axis od pocetne pozicije
    z_axis_init = np.array(uca3_init - lca3_init)
    z_axis_init = z_axis_init / np.linalg.norm(z_axis_init
                                               )
    # y axis od pocetne pozicije
    y_axis_init = np.cross(z_axis_init, x_axis_init)
    # translation je pomocu lca3_init
    translation_init = lca3_init
    # ova matrica je vec zapisana u inverznom obliku, koristi se za odredivanje koord wcn i spn u
    # lokalnom koord sus uprighta
    rot_matrix_loc = np.array([x_axis_init, y_axis_init, z_axis_init])
    # racuna lokalne koordinate tocaka prema formuli
    # wcn_init = rot_matrix_loc * wcn_local + translation_init, formula se samo obrne tako da racuna wcn_local, translation_init je referentna tocka
    # i to je u ovom slucaju lca3_init
    wcn_local = np.concatenate((np.matmul(rot_matrix_loc, wcn_init - translation_init), [1]))
    spn_local = np.concatenate((np.matmul(rot_matrix_loc, spn_init - translation_init), [1]))

    """func is function that follows from searching for common point between implicit equation of a sphere and explicit/parametric 
        equation of a spatial circle, by inputing x,y and z from spatial circle to implicit equation of a sphere.
        func_deriv is derivated func later used for finding theta by newton raphson method"""
    func = lambda ax, ay, az, bx, by, bz, cx, cy, cz, t: (cx + Rc * ax * np.cos(t) + Rc * bx * np.sin(t) - x) ** 2 + (cy + Rc * ay * np.cos(t) + Rc * by * np.sin(t) - y) ** 2 + (
            cz + Rc * az * np.cos(t) + Rc * bz * np.sin(t) - z) ** 2 - Rs ** 2
    func_der = lambda ax, ay, az, bx, by, bz, cx, cy, cz, t: 2 * Rc * (bx * np.cos(t) - ax * np.sin(t)) * (cx + Rc * ax * np.cos(t) + Rc * bx * np.sin(t) - x) + 2 * Rc * (
            by * np.cos(t) - ay * np.sin(t)) * (cy + Rc * ay * np.cos(t) + Rc * by * np.sin(t) - y) + 2 * Rc * (bz * np.cos(t) - az * np.sin(t)) * (cz + Rc * az * np.cos(t) + Rc * bz * np.sin(t) - z)

    tr2_storage = np.zeros(np.shape(lca3_travel))
    wcn_storage = np.zeros(np.shape(lca3_travel))
    spn_storage = np.zeros(np.shape(lca3_travel))

    index = 0
    for lca3_pos, uca3_pos in zip(lca3_travel, uca3_travel):
        # odreduje centar kruznice za trenutni hod kotaca
        tr12_pos = lca3_pos * (1 - t_param_dist) + uca3_pos * t_param_dist  # circle center
        # projicira prethodnu tr2 tocku na trenutnu os uprighta i kasnije se koristi za definiranje u_vectora koji je okomit na
        # os uprighta
        tr12_pos_fake = project_point2line(lca3_pos, uca3_pos, tr2_pos)
        # odreduje u i v vektore potrebne za crtanje kruznice uvijek u odnosu na iste tocke,
        # problematika je slijedeca, kruznica je definirana s 2 okomita vektora, sredistem i radijusom
        # pritom imamo sve te velicine osim jednog okomitog vektora, drugi je odreden samom osi uprighta
        # prvi vektor (u_vec) je problematican stoga sto moramo odrediti fiksnu tocku pomocu koje ce se definirati
        # za fiksne tocke su konacno uzete tr2 iz prethodne pozicije kotaca
        u_vec = (tr2_pos - tr12_pos_fake) / np.linalg.norm(tr2_pos - tr12_pos_fake)  # u unit vector for circle
        v_vec = np.cross((lca3_pos - uca3_pos) / np.linalg.norm(lca3_pos - uca3_pos), u_vec)

        # iniciranje prve iteracije newton-raphsonove metode, dalje nastavlja konvergirat unutar while loopa
        n1 = n0 - func(u_vec[0], u_vec[1], u_vec[2], v_vec[0], v_vec[1], v_vec[2], tr12_pos[0], tr12_pos[1], tr12_pos[2], n0) \
             / func_der(u_vec[0], u_vec[1], u_vec[2], v_vec[0], v_vec[1], v_vec[2], tr12_pos[0], tr12_pos[1], tr12_pos[2], n0)

        """finding theta using newton raphson method, when it finds one/converges it uses it to calculate tr2_pos"""
        while np.abs((n1 - n0) / n1) > prec:
            n0 = n1
            n1 = n0 - func(u_vec[0], u_vec[1], u_vec[2], v_vec[0], v_vec[1], v_vec[2], tr12_pos[0], tr12_pos[1], tr12_pos[2], n0) \
                 / func_der(u_vec[0], u_vec[1], u_vec[2], v_vec[0], v_vec[1], v_vec[2], tr12_pos[0], tr12_pos[1], tr12_pos[2], n0)

        n0 = 0
        # racuna trenutnu poziciju tr2 na temelju thete dobivene NR metodom
        tr2_pos = tr12_pos + Rc * np.cos(n1) * u_vec + Rc * np.sin(n1) * v_vec
        # current position
        transform_matrix_pos = np.eye(4)
        # radi stupce za matricu transformacije u novoj poziciji
        # prvi stupac, pomocu spone odreduje
        x_axis_pos = np.array(tr2_pos - project_point2line(lca3_pos, uca3_pos, tr2_pos))
        x_axis_pos /= np.linalg.norm(x_axis_pos)
        # treci stupac, pomocu osi uprighta odreduje
        z_axis_pos = np.array(uca3_pos - lca3_pos)
        z_axis_pos /= np.linalg.norm(z_axis_pos)
        # drugi stupac, pomocu prethodna 2 vektora odreduje
        y_axis_pos = np.cross(z_axis_pos, x_axis_pos)
        # cetvrti stupac- translacija zapravo je to lca3_pos

        # matrica transformacije za odredivanje pozicije nove tocke u glob lok sus, ova matrica istovremeno orijentira
        # i translatira tocku
        transform_matrix_pos[0:3, 0] = x_axis_pos
        transform_matrix_pos[0:3, 1] = y_axis_pos
        transform_matrix_pos[0:3, 2] = z_axis_pos
        transform_matrix_pos[0:3, 3] = lca3_pos

        # racuna trenutne pozicije spn i wcn u glob koord sus prema formuli
        # wcn_pos = transformation_matrix_pos * wcn_local
        wcn_pos = np.matmul(transform_matrix_pos, wcn_local)
        spn_pos = np.matmul(transform_matrix_pos, spn_local)

        # pohranjuje vrijednosti wcn i spn izracunate za trenutnu poziciju
        wcn_storage[index] = wcn_pos[:3]
        spn_storage[index] = spn_pos[:3]
        tr2_storage[index] = tr2_pos
        index += 1

    return tr2_storage, wcn_storage, spn_storage


# @nb.njit(cache=True)
def perpen_unit_vectors(p1, p2, p3):
    """project p3 to line p1,p2, thus creating p12, create u unit vector from p12 to p3
    and then create v unit vector from p12 to cross product of p2-p1 x p3 - p12
    output:
    u_vect je u smjeru od unutarnje fiktivne tocke ramena do vanjske tocke ramena,
    v_vect je okomit na u_vect i na liniju koja povezuje vanjske tocke ramena
    p12 projekcija tocke p3 na pravac p1p2
    line_u_lun razmak tocaka p12 i p3
    """
    p12 = project_point2line(p1, p2, p3)
    line_p12p3_len = np.linalg.norm(p12 - p3)
    x_axis = (p2 - p1) / np.linalg.norm(p2 - p1)  # moze biti kao X
    y_axis = (p3 - p12) / line_p12p3_len  # moze biti kao Y
    z_axis = np.cross(x_axis, y_axis)  # moze biti kao Z
    return x_axis, y_axis, z_axis, p12, line_p12p3_len


# @nb.njit(cache=True)
def camber_angle(wcn, spn):
    """racuna kut izmedu tocaka wcn i spn tako sto wcn tocki zada z koordinatu istu kao i spn tocka i onda racuna kut izmedu vektora
    (wcn-spn) i (wcn_z_promijenjen - spn)
    u gornjoj tocki camber bi trebao biti negativan a u donjoj pozitivan"""

    referenceVector2 = wcn - spn
    referenceVector2[:, -1] = 0
    measuredVector2 = wcn - spn
    angle = np.arccos((referenceVector2 * measuredVector2).sum(axis=1) / (np.linalg.norm(referenceVector2, axis=1) * np.linalg.norm(measuredVector2, axis=1)))

    return np.sign(spn[:, 2] - wcn[:, 2]) * np.rad2deg(angle)


# @nb.njit(cache=True)
def toe_angle(wcn, spn):
    """racuna kut izmedu tocaka wcn i spn tako sto napravi tocke [wcnx-spnx,wcny-spny,0], [0,wcny-spny,0] i [0,0,0] te potom racuna kut
    izemdu tocaka redom 1 3 2 gdje je 3. tocka vrh kuta
    paziti na predznake jer se sve odvija na negativnoj strani Y osi"""
    # point1 = wcn[:, 0:2] - spn[:, 0:2]  # np.array([wcn[:, 0] - spn[:, 0], wcn[:, 1] - spn[:, 1]]).T
    # point2 = np.array([wcn[:, 1] - spn[:, 1]])
    # betwee = (-wcn[:, 1] + spn[:, 1]) / np.linalg.norm(wcn[:, 0:2] - spn[:, 0:2])
    toe = np.arccos((-wcn[:, 1] + spn[:, 1]) / np.linalg.norm(wcn[:, 0:2] - spn[:, 0:2], axis=1))
    return np.sign(-wcn[:, 0] + spn[:, 0]) * np.rad2deg(toe)


# @nb.njit(cache=True)
def contact_patch_pos(wcn, spn, wheel_radius):
    """za unesen wcn i spn racuna gdje se nalazi tocka koja predstavlja contact patch, tj dodir gume s tlom koji
    je dodatno definiran s wheel_radius
    radi na nacin da trazi prvo vektorski produkt vektora (spn-wcn) i (0,0,-wcnz) a potom vektorski produkt prethodno
    nastalog vektora i ponovno vektora (spn-wcn). taj se konacni vektor potom svodi na jedinicnu duljinu, potom mnozi s
    radijusom gume te mu se konacno pribraja vektor wcn koji je sluzio kao referenca za odredivanje pozicije contact patcha"""

    bz = np.zeros(wcn.shape)
    bz[:, 2] = -wcn[:, 2]
    cp = np.cross(np.cross(spn - wcn, bz), spn - wcn).reshape((-1, 3))
    cp = cp/ np.linalg.norm(cp, axis=1).reshape((-1,1)) * wheel_radius + wcn #/ np.linalg.norm(cp, axis=1)
    # a=np.linalg.norm(cp, axis=1)
    # b=0
    # c=a.shape
    # u=0
    return cp


@nb.njit(cache=True)
def calc_roll_center_height_ref_pos(uca1, uca2, uca3, lca1, lca2, lca3, cp):
    """izracunava roll centre visinu samo u referentnoj poziciji koji je projiciran na ravninu paralelnu s
    YZ ravninom te koja prolazi kroz wcn tocku
    mora razlikovati 2 slucaja zbog djeljenja s nulom, kad su ramena paralelna (tad bi se djelilo s nulom) i kad nisu paralelna
    pomocu if-a provjerava vektore pravaca i odreduje koji je slucaj
    posebno racuna za svaki polozaj tocaka jer je moguce da u pocetnom polozaju ramena budu paralelna ali razlicitih duljina te mora racunati
    za 1. slucaj, dok kad se pomakne ovjes vise nece biti paralelna ramena zbog razlicitih duljina te racuna po drugom slucaju
    import sympy as sp
    from sympy import Point3D, simplify

    # preslikavanje iz 3d u 2d po principu (x3d, y3d, z3d) -> (-y3d, z3d)

    uca1x, uca1y, uca1z, uca2x, uca2y, uca2z, uca3x, uca3y, uca3z = sp.symbols("uca1[0], uca1[1], uca1[2], uca2[0], uca2[1], uca2[2], uca3[0], uca3[1], uca3[2]", real=True)
    lca1x, lca1y, lca1z, lca2x, lca2y, lca2z, lca3x, lca3y, lca3z = sp.symbols("lca1[0], lca1[1], lca1[2], lca2[0], lca2[1], lca2[2], lca3[0], lca3[1], lca3[2]", real=True)
    wcnx, wcny, wcnz = sp.symbols("wcn[0], wcn[1], wcn[2]", real=True)
    cpx, cpy, cpz = sp.symbols("cp[0], cp[1], cp[2]", real=True)  # contact patch
    # uca1x, uca1y, uca1z, uca2x, uca2y, uca2z, uca3x, uca3y, uca3z =[ 600, -300, 300, 700, -300, 300, 650, -600, 300]
    # lca1x, lca1y, lca1z, lca2x, lca2y, lca2z, lca3x, lca3y, lca3z = [600, -300, 100, 700, -300, 100, 650, -600, 100]
    # cpx, cpy, cpz = [650, -600, -10] # contact patch
    # wcnx, wcny, wcnz = [650, -600, 200]

    uca1 = [600, -300, 300]
    uca2 = [700, -300, 300]
    uca3 = [650, -600, 300]
    lca1 = [600, -300, 100]
    lca2 = [700, -300, 100]
    lca3 = [650, -600, 100]
    cp = [650, -600, -10]  # contact patch
    wcn = [650, -600, 200]


    xz_plane = sp.Plane(Point3D(0, 0, 0), Point3D(1, 0, 0), Point3D(0, 0, 1))
    uca_plane = sp.Plane(Point3D(uca1x, uca1y, uca1z), Point3D(uca2x, uca2y, uca2z), Point3D(uca3x, uca3y, uca3z))
    lca_plane = sp.Plane(Point3D(lca1x, lca1y, lca1z), Point3D(lca2x, lca2y, lca2z), Point3D(lca3x, lca3y, lca3z))
    yz_wcn_plane = sp.Plane(Point3D(cpx, 0, 1), Point3D(cpx, 1, 0), Point3D(cpx, 0, 0))
    #
    uca_line = sp.Plane.intersection(yz_wcn_plane, uca_plane)[0]
    lca_line = sp.Plane.intersection(yz_wcn_plane, lca_plane)[0]

    a_uca_formula = simplify(-uca_line.direction_ratio[2] / uca_line.direction_ratio[1])
    print("a_uca:")
    print(a_uca_formula, end=2 * "\n")

    b_uca_formula = simplify(sp.Plane.intersection(xz_plane, uca_line)[0][2])
    print("b_uca:")
    print(b_uca_formula, end=2 * "\n")

    a_lca_formula = simplify(-lca_line.direction_ratio[2] / lca_line.direction_ratio[1])
    print("a_lca:")
    print(a_lca_formula, end=2 * "\n")

    b_lca_formula = simplify(sp.Plane.intersection(xz_plane, lca_line)[0][2])
    print("b_lca:")
    print(b_lca_formula, end=2 * "\n")


    #
    cp_2d_ptx, cp_2d_pty = -cpy, cpz
    a_uca, b_uca, a_lca, b_lca = sp.symbols("a_uca, b_uca, a_lca, b_lca", real=True)

    x, y = sp.symbols("x, y", real=True)
    # #
    y_line_2d = sp.Line2D(sp.Point2D(0, 0), sp.Point2D(0, 1))
    # #

    uca_line_2d_eq = sp.Line(a_uca * x + b_uca - y)
    lca_line_2d_eq = sp.Line(a_lca * x + b_lca - y)
    # #
    # # provjera jesu li vektori paralelni, ako su slopeovi jednaki onda su paralelni
    uca_line_2d_slope = uca_line_2d_eq.slope
    lca_line_2d_slope = lca_line_2d_eq.slope
    check_if_parallel = uca_line_2d_slope - lca_line_2d_slope
    print("check if parallel")
    print(simplify(check_if_parallel), end="\n\n")
    # #
    # # # # 1. slucaj
    parallel_line_2d = sp.Line2D(sp.Point2D(cp_2d_ptx, cp_2d_pty), slope=uca_line_2d_slope)
    rc_point_parallel_z_coord = parallel_line_2d.intersection(y_line_2d)[0][1]
    # value to return in case of parallel control arms
    rc_height_parallel = rc_point_parallel_z_coord - cp_2d_pty
    print("value to return in case of parallel control arms")
    print(simplify(rc_height_parallel), end="\n\n")
    # #
    # # 2. slucaj
    instant_roll_centre_2d_pt = uca_line_2d_eq.intersection(lca_line_2d_eq)[0]
    print("instant roll centre X")
    print(instant_roll_centre_2d_pt[0], end="\n\n")
    print("instant roll centre Y")
    print(instant_roll_centre_2d_pt[1], end="\n\n")
    # #
    irc_2d_ptx, irc_2d_pty = sp.symbols("irc_2d_ptx, irc_2d_pty", real=True)
    non_parallel_line_2d = sp.Line2D(sp.Point2D(cp_2d_ptx, cp_2d_pty), sp.Point2D(irc_2d_ptx, irc_2d_pty))
    rc_point_non_parallel_z_coord = non_parallel_line_2d.intersection(y_line_2d)[0][1]
    # # value to return in case of non parallel control arms
    rc_height_non_parallel = rc_point_non_parallel_z_coord - cp_2d_pty
    print("value to return in case of non parallel control arms")
    print(simplify(rc_height_non_parallel))

    """
    # karakteristicne velicine za definiranje pravca u 2d prostoru
    a_uca = (-(uca1[0] - uca2[0]) * (uca1[2] - uca3[2]) + (uca1[0] - uca3[0]) * (uca1[2] - uca2[2])) / ((uca1[0] - uca2[0]) * (uca1[1] - uca3[1]) - (uca1[0] - uca3[0]) * (uca1[1] - uca2[1]))
    b_uca = (-cp[0] * uca1[1] * uca2[2] + cp[0] * uca1[1] * uca3[2] + cp[0] * uca1[2] * uca2[1] - cp[0] * uca1[2] * uca3[1] - cp[0] * uca2[1] * uca3[2] + cp[0] * uca2[2] * uca3[1] + uca1[0] * uca2[
        1] * uca3[2] - uca1[0] * uca2[2] * uca3[1] - uca1[1] * uca2[0] * uca3[2] + uca1[1] * uca2[2] * uca3[0] + uca1[2] * uca2[0] * uca3[1] - uca1[2] * uca2[1] * uca3[0]) / (
                    uca1[0] * uca2[1] - uca1[0] * uca3[1] - uca1[1] * uca2[0] + uca1[1] * uca3[0] + uca2[0] * uca3[1] - uca2[1] * uca3[0])
    a_lca = (-(lca1[0] - lca2[0]) * (lca1[2] - lca3[2]) + (lca1[0] - lca3[0]) * (lca1[2] - lca2[2])) / ((lca1[0] - lca2[0]) * (lca1[1] - lca3[1]) - (lca1[0] - lca3[0]) * (lca1[1] - lca2[1]))
    b_lca = (-cp[0] * lca1[1] * lca2[2] + cp[0] * lca1[1] * lca3[2] + cp[0] * lca1[2] * lca2[1] - cp[0] * lca1[2] * lca3[1] - cp[0] * lca2[1] * lca3[2] + cp[0] * lca2[2] * lca3[1] + lca1[0] * lca2[
        1] * lca3[2] - lca1[0] * lca2[2] * lca3[1] - lca1[1] * lca2[0] * lca3[2] + lca1[1] * lca2[2] * lca3[0] + lca1[2] * lca2[0] * lca3[1] - lca1[2] * lca2[1] * lca3[0]) / (
                    lca1[0] * lca2[1] - lca1[0] * lca3[1] - lca1[1] * lca2[0] + lca1[1] * lca3[0] + lca2[0] * lca3[1] - lca2[1] * lca3[0])
    check_if_parallel = a_uca - a_lca
    if check_if_parallel < 0.001:  # provjerava je li produkt vektora usmjerenja pravaca uca i lca manji od granicne vrijednosti, ako je znaci da su pravci paralelni
        return a_uca * cp[1]
    else:
        # instant roll centre
        irc_2d_ptx = (-b_lca + b_uca) / (a_lca - a_uca)
        irc_2d_pty = (a_lca * b_uca - a_uca * b_lca) / (a_lca - a_uca)
        a = 1
        # gleda koliko je roll centre point visok u odnosu na contact patch, tj. rcp[2] - cp[2] racuna konacno
        return -cp[1] * (cp[2] - irc_2d_pty) / (cp[1] + irc_2d_ptx)  # roll_centre_point_height


@nb.njit(cache=True)
def calc_caster_trail_ref_pos(uca3, lca3, cp):
    """
    input: uca3, lca3, cp koordinate u referentnoj poziciji u obliku -np.array([1,2,3]) ili lista [1,2,3]
    output: double
    racuna caster trail u referentnoj poziciji. ako prava od uca3 i lca3 probada tlo ispred contact patcha gledajuci u smjeru gibanja
    onda caster trail ima pozitivan predznak, u suprotnom ima negativan
    izracun slijedi iz slijedeceg sympy koda:
    import sympy as sp
    from sympy import Point3D

    uca3x, uca3z = sp.symbols("uca3x,  uca3z")
    lca3x, lca3z = sp.symbols("lca3x,  lca3z")
    cpx, cpz = sp.symbols("cpx, cpz")

    line_cp = sp.Line3D(Point3D(cpx, 0, cpz), Point3D(0, 0, cpz))
    line_ca = sp.Line3D(Point3D(uca3x, 0, uca3z), Point3D(lca3x, 0, lca3z))

    fin_point = line_ca.intersection(line_cp)
    print(fin_point)"""
    return cp[0] - (uca3[0] * (lca3[2] - uca3[2]) + (cp[2] - uca3[2]) * (lca3[0] - uca3[0])) / (lca3[2] - uca3[2])


@nb.njit(cache=True)
def calc_caster_angle_ref_pos(uca3, lca3):
    """racuna caster angle u referentnoj poziciji, ako je lca3X ispred uca3X gledano u smjeru
    gibanja tada je caster trail pozitivan sto uvijek trazimo"""
    return np.rad2deg(np.arctan((uca3[0] - lca3[0]) / (uca3[2] - lca3[2])))


@nb.njit(cache=True)
def calc_kingpin_angle_ref_pos(uca3, lca3):
    """racuna kingpin angle u referentnoj poziciji.
    kingpin angle je pozitivan ako je lca3y vise prema van nego uca3y"""
    return np.rad2deg(np.arctan((-lca3[1] + uca3[1]) / (uca3[2] - lca3[2])))


def calc_scrub_radius_ref_pos(uca3, lca3, cp):
    """racuna scrub radius u referentnoj poziciji.
    racuna se projiciranjem tocki na ravninu paralelnu s YZ ravninom.
    scrub radius je pozitivan ako contact patch je vise prema van od tocke koju definira
    pravac definiran s uca3 i lca3 i pod kojeg taj pravac presjeca.
    izracun slijedi iz slijedeceg sympy koda:
    import sympy as sp
    from sympy import Point3D

    uca3y, uca3z = sp.symbols("uca3y,  uca3z")
    lca3y, lca3z = sp.symbols("lca3y,  lca3z")
    cpy, cpz = sp.symbols("cpy, cpz")

    line_cp = sp.Line3D(Point3D(0, cpy, cpz), Point3D(0, 0, cpz))
    line_ca = sp.Line3D(Point3D(0, uca3y, uca3z), Point3D(0, lca3y, lca3z))

    fin_point = line_ca.intersection(line_cp)[0]
    scrub_radius = -cpy + fin_point[1]

    print(scrub_radius)"""
    return -cp[1] + (uca3[1] * (lca3[2] - uca3[2]) + (cp[2] - uca3[2]) * (lca3[1] - uca3[1])) / (lca3[2] - uca3[2])


def calc_anti_features_ref_pos(cg_height, wheelbase,
                               front_brake_bias, front_drive_bias,
                               uca1, uca2, uca3, lca1, lca2, lca3, wcn, cp,
                               front_susp=True,
                               outboard_brake=True, outboard_drive=True):
    """racuna anti dive i anti lift u referentnoj poziciji.
    koristi se samo za prednji ovjes za opisivanje ponasanja auta pri kocenju i ubrzanju
    cgh - center of gravity height in mm,
    wb - wheelbase in mm,
    fwd- front weight distribution, 0..1, 0- no mass on front wheels, 1- all mass on front wheels,
    wr - wheel radius mm,
    fbb - front_brake_bias, 0...1 range, 0 meaning that front wheels dont brake and 1 meaning that only front wheels brake
    fdb - front_drive_bias, 0...1 range, 0 meaning that front wheels dont contribute to acceleration and 1 meaning that only front wheels contribute to acceleration
    uca1, uca2, uca3, lca1, lca2, lca3, wcn, cp koordinate karakteristicnih tocaka ovjesa u referentnoj poziciji
    return:  drive znacajka, brake znacajka
    formule slijede iz sympy koda:
    import sympy as sp
    from sympy import Point3D, simplify, sqrt
    from sympy.matrices import Matrix

    with open("t1est_output.txt", "w") as f:

        print("####################################", file=f)
        print("####################################", file=f)
        print("\n", file=f)
        uca1x, uca1y, uca1z = sp.symbols("uca1[0], uca1[1], uca1[2]", real=True)
        uca2x, uca2y, uca2z = sp.symbols("uca2[0], uca2[1], uca2[2]", real=True)
        uca3x, uca3y, uca3z = sp.symbols("uca3[0], uca3[1], uca3[2]", real=True)
        lca1x, lca1y, lca1z = sp.symbols("lca1[0], lca1[1], lca1[2]", real=True)
        lca2x, lca2y, lca2z = sp.symbols("lca2[0], lca2[1], lca2[2]", real=True)
        lca3x, lca3y, lca3z = sp.symbols("lca3[0], lca3[1], lca3[2]", real=True)
        wcnx, wcny, wcnz = sp.symbols("wcn[0], wcn[1], wcn[2]", real=True)
        cpx, cpy, cpz = sp.symbols("cp[0], cp[1], cp[2]", real=True)  # contact patch
        ref_ptx, ref_pty, ref_ptz = sp.symbols("ref_pt[0], ref_pt[1], ref_pt[2]", real=True)  # reference point koji se mijenja s wcn ili cp ovisno o tome radi li se o inboard ili outboard
        plane_offset = sp.symbols("plane_offset", real=True)  # ako se racuna za rear onda je negativno, ako za front onda pozitivno

        # ne translatirano rear susp vulpes R
        # uca1x, uca1y, uca1z = [2040.563, -416.249, 275.203]
        # uca2x, uca2y, uca2z = [2241.481, -417.314, 270.739]
        # uca3x, uca3y, uca3z = [2153, -578, 315]
        # lca1x, lca1y, lca1z = [2038.666, -411.709, 132.316]
        # lca2x, lca2y, lca2z = [2241.147, -408.195, 126.205]
        # lca3x, lca3y, lca3z = [2135, -600, 140]
        # wcnx, wcny, wcnz = [2143.6, -620.5, 220.07]
        # cpx, cpy, cpz = [2143.6, -620.5, 220.07]
        # # cpx, cpy, cpz = [2143.6, -627.2131866, -9.83200906]
        # plane_offset = -100  # ako se racuna za rear onda je negativno, ako za front onda pozitivno

        # translatirano
        # uca1x, uca1y, uca1z = [546.963,	-395.749,	255.133]
        # uca2x, uca2y, uca2z = [747.881,	-396.814,	250.669]
        # uca3x, uca3y, uca3z = [659.4	,-557.5,	294.93]
        # lca1x, lca1y, lca1z = [545.066	,-391.209,	112.246]
        # lca2x, lca2y, lca2z = [747.547	,-387.695,	106.135]
        # lca3x, lca3y, lca3z = [641.4	,-579.5,	119.93]
        # wcnx, wcny, wcnz = [650,	-600,	200]
        # cpx, cpy, cpz = [650,	-606.7131866,	-29.90200906]

        xz_wcn_plane = sp.Plane(Point3D(0, wcny, 0), Point3D(1, wcny, 0), Point3D(0, wcny, 1))
        uca_plane = sp.Plane(Point3D(uca1x, uca1y, uca1z), Point3D(uca2x, uca2y, uca2z), Point3D(uca3x, uca3y, uca3z))
        lca_plane = sp.Plane(Point3D(lca1x, lca1y, lca1z), Point3D(lca2x, lca2y, lca2z), Point3D(lca3x, lca3y, lca3z))

        uca_line = sp.Plane.intersection(xz_wcn_plane, uca_plane)[0]
        lca_line = sp.Plane.intersection(xz_wcn_plane, lca_plane)[0]

        # provjera jesu li vektori paralelni
        uca_vec = Matrix(uca_line.direction_ratio).normalized()
        lca_vec = Matrix(lca_line.direction_ratio).normalized()
        # print(uca_vec)
        # print(lca_vec)

        check_if_parallel = simplify(uca_vec.cross(lca_vec)[1])
        print("check if parallel", file=f)
        print(check_if_parallel, file=f)

        final_plane_to_intersect_rear = sp.Plane(Point3D(wcnx + plane_offset, 0, 0), Point3D(wcnx + plane_offset, 1, 0),
                                                 Point3D(wcnx + plane_offset, 0, 1))

        # 1. slucaj - ako jesu paralelni
        # 1.1 outboard ili inboard odreduje ref_pt
        parallel_line_out_rear = sp.Line3D(Point3D(ref_ptx, 0, ref_ptz), direction_ratio=uca_line.direction_ratio)

        final_intersection_pt_out_parallel_rear = sp.Plane.intersection(final_plane_to_intersect_rear, parallel_line_out_rear)[0]
        print("\n*final point rear susp parallel outboard X:", file=f)
        print(simplify(final_intersection_pt_out_parallel_rear[0]).evalf(), file=f)
        print("*final point rear susp parallel outboard Y:", file=f)
        print(simplify(final_intersection_pt_out_parallel_rear[1]).evalf(), file=f)
        print("*final point rear susp parallel outboard Z:", file=f)
        print(simplify(final_intersection_pt_out_parallel_rear[2]).evalf(), file=f)


        # 2. slucaj - ako nisu paralelni
        instant_roll_centre_pt = uca_line.intersection(lca_line)[0]
        instant_roll_centre_pt_x = simplify(instant_roll_centre_pt[0])
        instant_roll_centre_pt_y = simplify(instant_roll_centre_pt[1])
        instant_roll_centre_pt_z = simplify(instant_roll_centre_pt[2])
        print("\n*instant roll centre tocka X:", file=f)
        print(instant_roll_centre_pt_x.evalf(), file=f)
        print("*instant roll centre tocka Y:", file=f)
        print(instant_roll_centre_pt_y.evalf(), file=f)
        print("*instant roll centre tocka Z:", file=f)
        print(instant_roll_centre_pt_z.evalf(), file=f)
        # Lr for rear suspension
        Lr = cpx - instant_roll_centre_pt_x
        print("\n*Lr iznosi:", file=f)
        print(Lr.evalf(), file=f)

        # 2.1 outboard ili inboard odreduje ref_pt
        hf_outboard = instant_roll_centre_pt_z - ref_ptz
        print("\n*hf_outboard:", file=f)
        print(hf_outboard.evalf(), file=f)

    """
    # ako je front_susp True onda se racuna za prednji ovjes anti znacajke te se neki izrazi mnoze s
    # -1 jer su svi izrazi za front i rear anti znacajke generalno isti, razlika je samo u par predznaka
    # ako je false mnozi s 1 i racuna za straznji ovjes
    if front_susp:
        front_rear_switch = -1
        drive_feature_multiplier = front_drive_bias
        brake_feature_multiplier = front_brake_bias
    else:
        front_rear_switch = 1
        drive_feature_multiplier = (1 - front_drive_bias)
        brake_feature_multiplier = (1 - front_brake_bias)
    # uca and lca 2d lines a nd b parameters
    a_uca = (-(uca1[1] - uca2[1]) * (uca1[2] - uca3[2]) + (uca1[1] - uca3[1]) * (uca1[2] - uca2[2])) / ((uca1[0] - uca2[0]) * (uca1[1] - uca3[1]) - (uca1[0] - uca3[0]) * (uca1[1] - uca2[1]))
    b_uca = (cp[1] * uca1[0] * uca2[2] - cp[1] * uca1[0] * uca3[2] - cp[1] * uca1[2] * uca2[0] + cp[1] * uca1[2] * uca3[0] + cp[1] * uca2[0] * uca3[2] - cp[1] * uca2[2] * uca3[0] + uca1[0] * uca2[1] *
             uca3[2] - uca1[0] * uca2[2] * uca3[1] - uca1[1] * uca2[0] * uca3[2] + uca1[1] * uca2[2] * uca3[0] + uca1[2] * uca2[0] * uca3[1] - uca1[2] * uca2[1] * uca3[0]) / (
                    uca1[0] * uca2[1] - uca1[0] * uca3[1] - uca1[1] * uca2[0] + uca1[1] * uca3[0] + uca2[0] * uca3[1] - uca2[1] * uca3[0])
    a_lca = (-(lca1[1] - lca2[1]) * (lca1[2] - lca3[2]) + (lca1[1] - lca3[1]) * (lca1[2] - lca2[2])) / ((lca1[0] - lca2[0]) * (lca1[1] - lca3[1]) - (lca1[0] - lca3[0]) * (lca1[1] - lca2[1]))
    b_lca = (cp[1] * lca1[0] * lca2[2] - cp[1] * lca1[0] * lca3[2] - cp[1] * lca1[2] * lca2[0] + cp[1] * lca1[2] * lca3[0] + cp[1] * lca2[0] * lca3[2] - cp[1] * lca2[2] * lca3[0] + lca1[0] * lca2[1] *
             lca3[2] - lca1[0] * lca2[2] * lca3[1] - lca1[1] * lca2[0] * lca3[2] + lca1[1] * lca2[2] * lca3[0] + lca1[2] * lca2[0] * lca3[1] - lca1[2] * lca2[1] * lca3[0]) / (
                    lca1[0] * lca2[1] - lca1[0] * lca3[1] - lca1[1] * lca2[0] + lca1[1] * lca3[0] + lca2[0] * lca3[1] - lca2[1] * lca3[0])

    check_if_parallel = -a_lca + a_uca

    # SLucaj paralelnih ramena
    if check_if_parallel < 0.001:  # ako su paralelni koristi jednu formulu, ako nisu onda drugu
        length_between_ref_pt_and_intersection_pt = 100  # koliko je plane offsetan od CP-a

        plane_offset = length_between_ref_pt_and_intersection_pt * 100  # ako se racuna za rear onda je konacna vrijednost pozitivno , ako za front onda negativno

        if outboard_brake == outboard_drive:  # i brake i drive je na istoj poziciji pa racuna samo jednu visinu, tj i jedno i drugo je inboard ili outboard
            ref_pt = cp if outboard_drive is True else wcn  # ako je outboard_drive True onda je referentna tocka za izracun anti znacajki contact patch, u suprtnom wcn
            h_drive = a_uca * (cp[0] - plane_offset - ref_pt[0])
            h_brake = h_drive

        else:  # brake i drive nisu na istoj poziciji, tj jedno je outboard, a drugo inboard pa racuna dvije visine

            h_cp = a_uca * (cp[0] - plane_offset - cp[0])
            h_wcn = a_uca * (cp[0] - plane_offset - wcn[0])
            if outboard_drive:
                h_drive = h_cp
                h_brake = h_wcn
            else:
                h_drive = h_wcn
                h_brake = h_cp

    # nisu paralelni
    else:
        intersection_pt_x = (-b_lca + b_uca) / (a_lca - a_uca)
        intersection_pt_z = (a_lca * b_uca - a_uca * b_lca) / (a_lca - a_uca)
        length_between_ref_pt_and_intersection_pt = (wcn[0] - intersection_pt_x) * front_rear_switch

        if outboard_brake == outboard_drive:  # i brake i drive je na istoj poziciji pa racuna samo jednu visinu, tj i jedno i drugo je inboard ili outboard
            ref_pt = cp if outboard_drive is True else wcn  # ako je outboard_drive True onda je referentna tocka za izracun anti znacajki contact patch, u suprtnom wcn
            h_drive = intersection_pt_z - ref_pt[2]
            h_brake = h_drive

        else:  # brake i drive nisu na istoj poziciji, tj jedno je outboard, a drugo inboard pa racuna dvije visine

            h_cp = intersection_pt_z - cp[2]
            h_wcn = intersection_pt_z - wcn[2]
            # ako je outboard drive onda je inboard brake
            if outboard_drive:
                h_drive = h_cp
                h_brake = h_wcn
            # suprotno
            else:
                h_drive = h_wcn
                h_brake = h_cp

        # izbacuje konacne vrijednosti anti znacajki
    anti_drive_feature = drive_feature_multiplier * h_drive / length_between_ref_pt_and_intersection_pt * wheelbase / cg_height * 100
    anti_brake_feature = brake_feature_multiplier * h_brake / length_between_ref_pt_and_intersection_pt * wheelbase / cg_height * 100
    return anti_drive_feature, anti_brake_feature


if __name__ == "__main__":
    print("a")
