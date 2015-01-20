﻿

// Interpolated values from the vertex shaders
in vec2 UV;

// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D diffuse;
uniform sampler2D normals;
uniform sampler2D positions;
uniform sampler2D uvs;
uniform sampler2D matDataA;
uniform sampler2D matDataB;

//SSAO Uniforms
uniform sampler2D rnm;		//random normal data
uniform float totStrength;
uniform float strength;
uniform float offset;
uniform float falloff;
uniform vec2 rad;
uniform mat4 ProjectionMat;

in vec3 cameraPosition;
uniform vec3 lightColor0;
uniform vec3 lightDirection0;


#define SAMPLES 128 // 10 is good
const float invSamples = 1/SAMPLES;

//Hemispherical directions
const vec3 sDirs[] = vec3[](
vec3(-0.5678512477498036,-0.7015190467974507,-0.6291680251812255)
,
vec3(0.2559505929558622,0.19854878222139216,0.7901353500878895)
,
vec3(-0.12463146929343137,0.7524437455386422,-0.16340918667692114)
,
vec3(0.09572890820750907,0.01895577984112957,0.9809533090880259)
,
vec3(0.22108483063874,0.12458216757963125,0.8712015623652007)
,
vec3(0.1398542808101351,0.042226672598119346,0.9573153765207423)
,
vec3(0.038305215623933,0.6860457221394631,0.05574795518029304)
,
vec3(0.07399200781840194,-0.011167669320037944,0.9888009318819196)
,
vec3(-0.6280913288732605,-0.6691073709874834,-0.6844067826311203)
,
vec3(0.24074753026668733,0.15996217223906395,0.832905460246101)
,
vec3(-0.790398056016218,-0.5373640332015998,-0.8269783822658128)
,
vec3(0.2542917088626541,0.41993280954950923,0.5179843245351335)
,
vec3(-0.463302067476208,-0.7417716462497047,-0.5297479618154515)
,
vec3(0.06059639148091184,0.007440666731900159,0.9925454276361537)
,
vec3(0.038125215983233325,-0.0029220019471365034,0.9970758596217055)
,
vec3(-0.14705309226427465,-0.7574113809207721,-0.19059322378559213)
,
vec3(-0.3562500893902665,-0.7643956949893899,-0.4224298094177706)
,
vec3(-0.14444964154950812,0.7568797614732944,-0.18746534454330446)
,
vec3(0.05434081755904828,0.6760057210944636,0.08012668118913734)
,
vec3(-0.9420212924071856,0.29868386452366924,-0.9532323325505958)
,
vec3(0.15158626023075678,-0.05037723318025191,0.9489674801727116)
,
vec3(-0.9736408336333576,-0.2035695742826877,-0.9788340889837887)
,
vec3(-0.46052934378443905,0.7425863893785278,-0.5270436443535292)
,
vec3(-0.9598138865955168,-0.2501820676514807,-0.9676675277519237)
,
vec3(0.1182546740035609,0.02948569407146858,0.9702928518428706)
,
vec3(0.22810932387173508,0.13598852964314148,0.8589465123365526)
,
vec3(-0.8042138910345578,0.5221199778243061,-0.8387385075523947)
,
vec3(0.20676164603926545,-0.10436944720976005,0.8927132804325253)
,
vec3(0.05227725906497396,-0.6773414802464766,0.07695121464433125)
,
vec3(-0.37491259286876805,0.7617333619981688,-0.4415943341452317)
,
vec3(-0.18486160134961174,0.7638324246220688,-0.23522756911514228)
,
vec3(-0.37771059084549324,-0.7612881888678537,-0.44444999389309714)
,
vec3(-0.4419918876717415,-0.7477141397145152,-0.5088665269932937)
,
vec3(-0.33202344487700153,0.7670583670130007,-0.3972362127052849)
,
vec3(-0.8900299353038619,-0.4039216602285826,-0.9106119866776224)
,
vec3(0.26342277407512704,0.22579636885951737,0.7592488838168426)
,
vec3(0.26205946249592643,-0.39124150467891267,0.556509846265853)
,
vec3(0.1270511265898216,0.6191539705459892,0.20101274397878696)
,
vec3(0.27011902942556776,0.3432911098809109,0.6183738476378432)
,
vec3(0.27211197933932174,0.29802481902154815,0.67427255589442)
,
vec3(-0.9890100661947083,-0.13212238460015768,-0.9911944710937866)
,
vec3(0.08302762024655475,-0.655990168474184,0.1255666262828113)
,
vec3(0.25996143487272716,0.21203377412291136,0.774923462020214)
,
vec3(0.05018613110548398,-0.005082480222147895,0.9949110412789093)
,
vec3(-0.7740529665832683,-0.5543817477155994,-0.8129942345531218)
,
vec3(-0.967080384665315,-0.2269939513415726,-0.973541448700148)
,
vec3(-0.8758270414424913,0.42702770316942734,-0.8988513315921279)
,
vec3(-0.9518502175760292,-0.27311001440073207,-0.9612158333310044)
,
vec3(0.2470206749294055,-0.1741519633468714,0.8173037596397018)
,
vec3(-0.7572364953258351,-0.5708372998317466,-0.7985246654651058)
,
vec3(0.09722179386001266,-0.01957342199979768,0.9803296078997181)
,
vec3(0.2520253804648889,-0.18706326539543364,0.802981084682251)
,
vec3(-0.8626277670829207,0.44700204009113836,-0.8878749767762105)
,
vec3(0.2371948399632791,0.4655826217862721,0.4539428603708309)
,
vec3(-0.8364102417101226,-0.48310045272564833,-0.8659362797226239)
,
vec3(-0.1875281756698732,-0.7641952190695798,-0.23832229903774757)
,
vec3(0.1701385354689845,0.06531354378580911,0.9335740394932166)
,
vec3(-0.2261566620593915,0.7681583198355789,-0.2824280802529307)
,
vec3(0.1383447952100684,-0.6082511424255397,0.22178253075262033)
,
vec3(-0.978765850539732,0.18302541397884106,-0.9829617846897822)
,
vec3(-0.771887159908607,0.5565589922436441,-0.811135398958072)
,
vec3(-0.03028186849262486,0.7211518629138045,-0.04195400188691083)
,
vec3(-0.911795483957553,0.36455018108171794,-0.9285356781842034)
,
vec3(0.26791969019766376,-0.3605581295957774,0.596433749573565)
,
vec3(-0.35345392964641953,0.7647487044685198,-0.41954052273755116)
,
vec3(-0.7199336365577035,0.6039502991225664,-0.7661208097148737)
,
vec3(-0.9132308008900837,-0.36175440412096665,-0.9297134891905188)
,
vec3(0.09765078026367705,-0.6446547642140134,0.14976912018013996)
,
vec3(-0.48451202207241406,-0.7351221995204551,-0.5503130955209826)
,
vec3(0.17138957779708255,-0.06642094896954676,0.9324277403210453)
,
vec3(0.13058735290123236,-0.03641950367742523,0.963241126028278)
,
vec3(0.1861002347963292,0.5530965225623625,0.31890187867634623)
,
vec3(-0.6643281870171007,0.6458819824747737,-0.7169909507019437)
,
vec3(0.26568165318106335,0.3738870935695012,0.5792434008502563)
,
vec3(0.2581806296916904,-0.406596889563469,0.5360434636986288)
,
vec3(0.12918124385320903,0.0355842592318075,0.9640919334629225)
,
vec3(0.08569042996279229,-0.015083291797954014,0.9848592890426592)
,
vec3(-0.70416454395334,-0.6166829950490612,-0.7522912426873914)
,
vec3(0.1795858123643725,0.07402392442096244,0.924538789221496)
,
vec3(0.24922652540178322,0.4352256034245404,0.4969296263197979)
,
vec3(-0.012503486309647734,0.7131679147357999,-0.017529623557009252)
,
vec3(0.10856334615362523,-0.024631349085593855,0.9752145930283035)
,
vec3(0.2354888012361904,-0.14935391411098617,0.8444768656641404)
,
vec3(-0.5238118414327049,0.7208030481991795,-0.5878717590366997)
,
vec3(-0.24707733568114545,0.7693139772337301,-0.3057824107489473)
,
vec3(-0.42060653356907857,-0.7529480548976228,-0.48768125891022035)
,
vec3(0.22899108353427067,-0.13750471822771515,0.8573110722538346)
,
vec3(0.06989852089983821,0.6654963667340308,0.10445756527963836)
,
vec3(0.08496281705629516,0.6545366908858566,0.12872608000410196)
,
vec3(0.024404193015894617,0.0011936222923515831,0.9988060212581323)
,
vec3(-0.06943298526905732,-0.7362889618482863,-0.0938847495660009)
,
vec3(0.26891259608736057,-0.25594434942884764,0.724357011322)
,
vec3(-0.5055967572433286,-0.7277683310095435,-0.5705496491107718)
,
vec3(-0.08836763484619434,-0.7424838345145288,-0.1181821668074169)
,
vec3(-0.9794872152828569,-0.17993315241501565,-0.9835422884811417)
,
vec3(-0.9992318633728302,-0.035048473838541205,-0.999385424595899)
,
vec3(-0.6856707946270865,-0.630745719576418,-0.7359692027370269)
,
vec3(0.22927802788239499,-0.48254615763132375,0.4291615833712105)
,
vec3(0.2686517031103758,0.25407003103487164,0.7265493634916678)
,
vec3(0.1253223656321553,-0.6207645167568955,0.19789143881587684)
,
vec3(0.27039864912455003,0.2685378889029786,0.7095439455503)
,
vec3(0.18079140524213197,-0.07519546782265289,0.9233202188190152)
,
vec3(-0.3106459598451855,0.7686653682637399,-0.37469472147233474)
,
vec3(-0.848849744646116,0.4665172627740711,-0.8763684909039762)
,
vec3(-0.9588151064545344,0.25318607379032115,-0.9668591926535561)
,
vec3(0.14000143683373897,0.6065936919435232,0.22488738115762943)
,
vec3(0.2604440345594503,-0.21381249396120694,0.7729062445769694)
,
vec3(-0.2893461501473376,0.7695736218414149,-0.3519295080783977)
,
vec3(0.1984964664852805,-0.09418398654702244,0.9034570589419364)
,
vec3(0.002625512193729607,-0.7057854677305857,0.003719960452077853)
,
vec3(0.012213947428048448,0.0002985169660560673,0.9997014607516916)
,
vec3(-0.2921173983058792,-0.7694946721173244,-0.354909249619888)
,
vec3(0.17401924995165388,-0.5686958676825894,0.29260462145402116)
,
vec3(0.1070964839047277,0.02394010280811609,0.9759144292255625)
,
vec3(0.25872219214514786,0.4045960387439017,0.5387297454487048)
,
vec3(-0.8506779862964784,-0.4640000780843798,-0.8778982176634844)
,
vec3(0.2653051173601872,-0.3758872441532687,0.5766439487707166)
,
vec3(0.2716615822100468,-0.2851207788585436,0.689812252428463)
,
vec3(-0.05084660311627991,-0.7295068614010871,-0.0695312757594588)
,
vec3(-0.9338711070791523,-0.31809148538171356,-0.946594875419173)
,
vec3(0.02599025681447952,-0.0013541957407978474,0.998645345409226)
,
vec3(0.20439922124806256,-0.5265866263372441,0.3618549666318907)
,
vec3(0.25140252283659004,0.1853619131512524,0.8048754653286108)
,
vec3(-0.20541026240090682,0.7663283781200247,-0.25890511802335436)
,
vec3(0.2720626431195808,0.3129985562821003,0.656027643968217)
,
vec3(-0.39917107965315307,-0.7574726744811725,-0.46620480683424875)
,
vec3(0.272011176064968,-0.31495998871851877,0.6536202512043671)
,
vec3(0.013804845700628905,-0.0003814020891835407,0.9996185615352564)
,
vec3(-0.3134287259686015,-0.7684955871870505,-0.3776460675765403)
,
vec3(0.24273750688345044,-0.4524285697309067,0.47277378408650594)
,
vec3(0.2699061656850496,-0.3452780697065066,0.6158674326098866)
,
vec3(-0.9952626863930379,0.08692525921056608,-0.9962076312302216)
,
vec3(0.2663231041192925,0.23981558956119273,0.743120974431364)
,
vec3(-0.7222711983128626,-0.602001967558644,-0.7681641057135542)
,
vec3(0.17545106890037684,0.5669148152357353,0.29564902937589455)
,
vec3(-0.1671702923359792,-0.7611274503361983,-0.21452180458995787)
,
vec3(0.19487503099137735,-0.5408688058064913,0.33896931440313943)
,
vec3(0.24352711309595118,0.4504502571117306,0.475578222110255)
,
vec3(-0.27090506125408925,-0.7697991356697778,-0.3319605229820374)
,
vec3(0.23023196980755511,0.4805985960478241,0.4320364591107868)
,
vec3(-0.6056718276510872,0.6820311578883238,-0.6640097262813922)
,
vec3(0.14122574452337225,-0.043132094594951056,0.9563898229993427)
,
vec3(-0.8196250769999635,0.5040944285672417,-0.8517929195194602)
,
vec3(0.06214951295415545,-0.007832404860689427,0.992152182947319)
,
vec3(0.1973893620944,0.09288953956883653,0.9048179463413109)
,
vec3(-0.6476113567889328,-0.656968757899118,-0.7020168365950251)
,
vec3(0.11355278476529729,0.6313451685611934,0.17701808641296887)
,
vec3(0.04861896883715429,0.004767364860291447,0.9952269362030014)
,
vec3(-0.032628513546367525,-0.722152008420259,-0.045136286323318425)
,
vec3(-0.8884315203911941,0.40661398694405415,-0.9092910016062965)
,
vec3(-0.877504135629247,-0.4243911673006266,-0.9002427418584407)
,
vec3(-0.9956023661633586,-0.08376031408486095,-0.9964797234513458)
,
vec3(-0.9839854047239364,0.15922449011344253,-0.9871594299232294)
,
vec3(0.24851951189823734,-0.43721457820269977,0.49416292962579433)
,
vec3(0.1502522247541956,0.04940298958599784,0.9499672271527609)
,
vec3(0.21361183532511013,0.11358593870575329,0.8829364366747418)
,
vec3(-0.7376859181525094,0.5887420087935219,-0.7815953335174846)
,
vec3(0.22264166010903605,0.4954741588602436,0.4098720981712532)
,
vec3(0.25366741557703254,-0.42192929706466226,0.515257021105978)
,
vec3(-0.9884782432263651,0.13525751148194096,-0.9907676634883369)
,
vec3(-0.9326058823017869,0.32097942896133236,-0.945563051040475)
,
vec3(-0.502856253283918,0.7287667531071269,-0.567930783802088)
,
vec3(-0.9431991038996397,-0.2957542209430232,-0.9541902176053957)
,
vec3(-0.9922388706752487,0.1111493995229982,-0.9937843309864325)
,
vec3(0.11969259730778449,-0.030249510786358673,0.969517298493805)
,
vec3(-0.9999294414630382,-0.010624895379632782,-0.9999435526128234)
,
vec3(-0.8643815007312999,-0.4444239101597548,-0.8893359814563601)
,
vec3(-0.821595587953213,-0.5017064328675334,-0.8534573098497008)
,
vec3(-0.7882948284130453,0.5396136719231084,-0.8251833028581851)
,
vec3(-0.972825963438188,0.20663557756691353,-0.978177234111701)
,
vec3(0.27140023620327014,0.32809510171214373,0.6373910320426146)
,
vec3(-0.04845050328669619,0.7285802728069743,-0.06635333038443872)
,
vec3(-0.9507612569171204,0.2760783622664271,-0.9603324595326709)
,
vec3(-0.10762999293562707,-0.748077733012988,-0.14240902001834863)
,
vec3(-0.5881874511924012,-0.6913908243829153,-0.6479714995622012)
,
vec3(0.21461079452600504,-0.11499550366451279,0.8814364820197237)
,
vec3(-0.9238776098178695,-0.3400990046606068,-0.9384343417878299)
,
vec3(0.26665927283299906,-0.241660844163497,0.7409857372207244)
,
vec3(0.1508089589736476,-0.5953873170790772,0.2455412011093262)
,
vec3(0.2625184002883643,0.3892394356975508,0.5591535024157807)
,
vec3(0.036180540623705744,-0.6873189262781432,0.05256732412007339)
,
vec3(0.18474581913779137,-0.5549129182747811,0.3158812708857452)
,
vec3(0.004865841466091539,0.7046443875599527,0.006905221334171997)
,
vec3(0.2638328795014925,-0.22760973937690734,0.7571720364694545)
,
vec3(0.11175349042146862,-0.6329060453972949,0.1738821901563519)
,
vec3(-0.10510122937710228,0.7473830757603048,-0.13925546069902345)
,
vec3(-0.9977889162889512,-0.059435435953465544,-0.9982305850321163)
,
vec3(0.16164777452291373,-0.058144065535089565,0.9409785292696808)
,
vec3(-0.6255245148138243,0.670641136741654,-0.6820809058464167)
,
vec3(-0.9846123819848879,-0.15610917735934,-0.9876632360275295)
,
vec3(0.036547067851899574,0.0026839798684762488,0.9973142161669886)
,
vec3(-0.5445957461998047,0.7121420768103015,-0.6074617286852226)
,
vec3(-0.5472910741193063,-0.7109619710224757,-0.6099888881016553)
,
vec3(-0.9975459905093479,0.06261034641609067,-0.9980361173192377)
,
vec3(0.24633024957838148,0.17249320211065494,0.8191350067365301)
,
vec3(0.271268412283263,-0.3300708276637038,0.6349333944454304)
,
vec3(0.2216062080957796,-0.49740165307544937,0.40696456810244075)
,
vec3(-0.06699000032519063,0.7354378697239071,-0.09071304073521516)
,
vec3(-0.9004258687702157,0.38578119654477927,-0.9191877535162388)
,
vec3(-0.24981603091359156,-0.7694145473187219,-0.30881358985418855)
,
vec3(0.21442795870778406,0.5101854023947017,0.3874630114155384)
,
vec3(0.27214007335733165,-0.29996892804070213,0.6719168453663563)
,
vec3(-0.2681485131470525,0.7697879301914625,-0.32895416514286213)
,
vec3(0.07245596256306777,0.010699893470713028,0.9892712915375494)
,
vec3(0.18867587383995424,0.0832205279423771,0.9149515167194434)
,
vec3(-0.9019436589518649,-0.3830357664852213,-0.9204375246607976)
,
vec3(-0.2288743850109025,-0.7683473506637922,-0.2854822707723748)
,
vec3(-0.5651833191695192,0.7027889539054668,-0.6266889959980301)
,
vec3(-0.83450971233481,0.48555423192906894,-0.8643387442507106)
,
vec3(-0.7017752776192967,0.6185514413109429,-0.7501888516509716)
,
vec3(-0.9661730050960419,0.23003052339702587,-0.9728086349412518)
,
vec3(0.18983346513268473,-0.08445445033448107,0.9136614026688369)
,
vec3(-0.6082754666614485,-0.6805843068312559,-0.6663880840943679)
,
vec3(-0.9926748887790708,-0.10799772134241267,-0.9941338852553881)
,
vec3(0.06789815270914043,-0.6668922410945926,0.10128915925303646)
,
vec3(0.09951891989038517,0.6431462640851848,0.15291773515424137)
,
vec3(0.2055957992838546,0.5247085647319819,0.36482257882747304)
,
vec3(-0.6832326979712956,0.6325326603429564,-0.7338089719553471)
,
vec3(-0.6450844373782395,0.6585882503030281,-0.6997448295696076)
,
vec3(-0.9998807967852189,0.013809784157684734,-0.9999046358366522)
,
vec3(-0.9990859888770935,0.03823003506875091,-0.9992686975037558)
,
vec3(0.2363228853969762,-0.4675471271371048,0.4511023554869754)
,
vec3(-0.08588040597000866,0.7417100848784716,-0.11501858828000588)
,
vec3(-0.16453380721070437,0.7606795723079139,-0.21140957088360016)
,
vec3(-0.43920794395560075,0.7484365894663099,-0.5061218929713354)
,
vec3(0.08417447261964887,0.01454017700992789,0.9854064828234479)
,
vec3(-0.3963745003223641,0.7580102209271846,-0.4633844790717664)
,
vec3(0.268214162830329,0.3585630211055473,0.5989874455057771)
,
vec3(0.16035389812942882,0.057102460229123624,0.9420518727809563)
,
vec3(0.2565041430931969,-0.20029014225131364,0.7881789669859466)
,
vec3(0.20570744357168655,0.10301642270613409,0.894144128624065)
,
vec3(0.21331158963191665,-0.5120897228104844,0.3845245630411721)
,
vec3(0.021807926107821004,0.6955979030487266,0.031335943266180236)
,
vec3(0.1642119059727953,0.580452084790858,0.272219654397457)
,
vec3(0.2346688344919932,0.1477878080571847,0.8461786038156445)
,
vec3(0.27155483455170526,0.28319698929062814,0.6921148741766397)
,
vec3(-0.33481452180904536,-0.7667971939041219,-0.4001574011869108)
,
vec3(-0.6668121849874611,-0.6441780620705864,-0.7192077314015362)
,
vec3(-0.4178146377627297,0.7535780663052035,-0.48489794709018147)
,
vec3(-0.48175378138805663,0.7360289727807996,-0.5476507093089011)
,
vec3(-0.9225265152589528,0.3429423067031313,-0.9373291941653827)
,
vec3(0.019624298436720426,-0.6968061879191281,0.028152046776958754)
,
vec3(-0.12719899376141708,-0.753057659093203,-0.16655084386570318)
,
vec3(-0.7550107275214578,0.5729401304710792,-0.7966031835533963)
,
vec3(-0.5265313685185916,-0.7197134762201215,-0.5904455397738246)
,
vec3(-0.20810393118877793,-0.7666048538892349,-0.26198049636551757)
,
vec3(-0.014798181801436215,-0.7142395867676128,-0.02071434698119725)
,
vec3(-0.8062519946098389,-0.5198000583611929,-0.8404687589692861)
,
vec3(-0.7399689596327435,-0.5867155154035016,-0.7835783144703227)
,
vec3(0.15239199949148874,0.5936857813965861,0.24862774291702225)
,
vec3(0.196150995640271,0.5390200623799412,0.34196431852250964)
,
vec3(-0.5855500957679902,0.6927496847633551,-0.6455420807872607)
,
vec3(0.16270384227710077,-0.5821948365116786,0.26915326409481627)
,
vec3(0.24150368923551518,-0.1615758275148291,0.8311384400970678)
,
vec3(0.22202628219097276,-0.12604614868280237,0.8696333968373748)
,
vec3(0.27058303270022893,-0.2704384009359172,0.7072957874279419)
);



float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float getDepthDiff(float length, vec3 currentPos, vec3 currentNorm, int i, out vec2 sampleUV)
{
		vec3 samplePos = currentPos + (currentNorm * sDirs[i] * length);	//Calculate the sample position	
		sampleUV = UV + (currentNorm.xy * sDirs[i].xy * length);			//Calculate the equivalent UV position

		//Test if this point is an occluder
		float actualLDepth = texture2D(uvs, sampleUV).z;
		float depthDiff = 50 * max(0.0, samplePos.z - actualLDepth)/currentPos.z;		//absolute value
		return depthDiff;
}

void main(){

	vec3 currentPos = 2.0 * texture2D(positions, UV).rgb - 1.0f;
	vec3 currentNorm = 2.0 * texture2D(normals, UV).rgb - 1.0f;
	float currentDepth = texture2D(uvs, UV).z;
	
	vec4 random = texture2D(rnm, UV).rgba;
	vec3 currentCol = texture2D(diffuse, UV).rgb;

	currentPos.z = currentDepth;

	vec3 cameraPos = -normalize((ProjectionMat * vec4(cameraPosition, 1)).xyz - currentPos);

	float ao = 0.0;
	vec3 aCol = vec3(0);
	//int i = 1;
	for(int i = 0; i < SAMPLES; i+=4){

		vec2 sampleUV;
		
		random.x = rand(random.xz);
		random.z = rand(random.yx);
		random.y = rand(random.zy);
		random.w = rand(random.xy);
		
		float depthDiff = getDepthDiff(random.x * 0.005, currentPos, currentNorm, i, sampleUV);
		//ao += 1/(1 + depthDiff * depthDiff);
		depthDiff += getDepthDiff(random.y * 0.01, currentPos, currentNorm, i + 1, sampleUV);
		//ao += 1/(1 + depthDiff * depthDiff);
		depthDiff += getDepthDiff(random.z * 0.02, currentPos, currentNorm, i + 2, sampleUV);
		//ao += 1/(1 + depthDiff * depthDiff);
		depthDiff += getDepthDiff(random.w * 0.04, currentPos, currentNorm, i + 3, sampleUV);
		//ao += 1/(1 + depthDiff * depthDiff);
		
		ao += 6/(1.5 + depthDiff * depthDiff);
		//ao += 2/(0.5 + depthDiff * depthDiff);
	}

	//color.rgb = vec3(pow(ao/SAMPLES, 2.0));
	//color.rgb = aColor/SAMPLES;
	color.rgb = aCol/SAMPLES;
	color.a = ao / SAMPLES; //max
	//color.a = 0.5;
}
