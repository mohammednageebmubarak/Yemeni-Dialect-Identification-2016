# التعرّف الآلي على اللهجات العربية المنطوقة
## دراسة حالة اللهجات اليمنية

[English](README.md) · [الرسالة الكاملة (PDF، 113 صفحة)](original-2016/docs/Full-Thesis-2016.pdf) · [ملف الاستشهاد بالأرشيف](CITATION.cff)

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.24600512.svg)](https://doi.org/10.5281/zenodo.24600512) · [الأرشيف البحثي على Zenodo (v1.0.0)](https://doi.org/10.5281/zenodo.24600512)

مشروع تخرج في قسم علوم وهندسة الحاسوب، كلية الهندسة، جامعة عدن، فبراير 2016. درس الفريق إمكانية التمييز بين خمس لهجات يمنية (العدنية والضالعية والحضرمية واللحجية والصنعانية) من خلال نطق عبارة «السلام عليكم». وقارن معاملات MFCC وتحويلات المويجات المتقطعة Haar وDB4 باستخدام مصنّفات تعلّم خاضع للإشراف.

## النتائج التاريخية

أبلغت الرسالة عن أعلى دقة قدرها **95٪** لكل من Naive Bayes وSVM عند استخدام DB4 مع تقطيع قيم السمات إلى فئات، كما يظهر في الجدول السابع. قُبل 80 تسجيلًا من أصل 125. العينة صغيرة وغير متوازنة، وتضم أصوات رجال فقط وعبارة واحدة. ولا توضح المواد المتاحة ما إذا كان اختيار السمات وتقطيع قيمها قد أُجريا داخل كل طيّة من طيّات التحقق. التسجيلات الأصلية والنماذج المدرّبة غير متاحة، ولم تُعَد هذه التجارب بصورة مستقلة.

[النتائج كاملة (CSV)](original-2016/results/reported-results.csv) · [أعداد التسجيلات](original-2016/dataset/recording-counts.csv) · [مخطط التصنيف الأصلي](original-2016/screenshots/p05-architecture.png) · [صور واجهات التطبيق](original-2016/screenshots/)

## فريق البحث الأصلي — 2016

<table>
  <tr>
    <td align="center" width="33%">
      <a href="https://github.com/alibaowain"><img src="https://github.com/alibaowain.png?size=160" width="100" height="100" alt="Ali A. Baowain — GitHub avatar"></a><br>
      <strong>Ali A. Baowain</strong><br>
      <a href="https://github.com/alibaowain">@alibaowain</a><br>
      <sub>باحث أصلي</sub>
    </td>
    <td align="center" width="33%">
      <a href="https://github.com/mohammednageebmubarak"><img src="https://github.com/mohammednageebmubarak.png?size=160" width="100" height="100" alt="Mohammed N. Mubark — GitHub avatar"></a><br>
      <strong>Mohammed N. Mubark</strong><br>
      <a href="https://github.com/mohammednageebmubarak">@mohammednageebmubarak</a><br>
      <sub>باحث أصلي</sub>
    </td>
    <td align="center" width="33%">
      <strong>Mohammed N. Obaid</strong><br>
      <sub>باحث أصلي</sub>
    </td>
  </tr>
  <tr>
    <td align="center" width="33%">
      <strong>Ahmed A. Sallam</strong><br>
      <sub>باحث أصلي</sub>
    </td>
    <td align="center" width="33%">
      <a href="https://github.com/ahmedrashadabdo"><img src="https://github.com/ahmedrashadabdo.png?size=160" width="100" height="100" alt="Ahmed R. Abdo — GitHub avatar"></a><br>
      <strong>Ahmed R. Abdo</strong><br>
      <a href="https://github.com/ahmedrashadabdo">@ahmedrashadabdo</a><br>
      <sub>باحث أصلي</sub>
    </td>
    <td align="center" width="33%">
      <a href="https://github.com/alHaitham2020"><img src="https://github.com/alHaitham2020.png?size=160" width="100" height="100" alt="Haytham Y. Al-Areaqi — GitHub avatar"></a><br>
      <strong>Haytham Y. Al-Areaqi</strong><br>
      <a href="https://github.com/alHaitham2020">@alHaitham2020</a><br>
      <sub>باحث أصلي</sub>
    </td>
  </tr>
</table>

**المشرف الأكاديمي:** Dr. Ibrahim Nani. تَرِد أسماء الباحثين الستة بترتيب صفحة عنوان الرسالة الأصلية.

[سجل المؤلفين](original-2016/AUTHORS.md) · [بيانات الاستشهاد](original-2016/CITATION.cff)

## مواد المستودع

- [أرشيف 2016](original-2016/README.md) يضم الرسالة بالفَهارس المصحّحة ومقتطفات من الشفرة والنتائج التاريخية.
- [التطبيق المعاد بناؤه](reconstructed/README.md) يضم شفرة كُتبت لاحقًا استنادًا إلى الرسالة. [حالة التحقق](reconstructed/VALIDATION.md)

## حقوق البحث والاستشهاد

تعود ملكية البحث الأصلي لعام 2016 إلى الباحثين الستة المذكورين في صفحة عنوان الرسالة. عند الاستفادة من منهجية البحث أو نتائجه، يُرجى الاستشهاد بالرسالة وذكر جميع الباحثين. ونرحّب بالتواصل معهم لمناقشة فرص التعاون عند تطوير البحث تطويرًا جوهريًا، على أن يُنسب تأليف العمل الجديد بحسب المساهمة الفعلية لكل مشارك.

لا يحدد المستودع ترخيصًا مفتوح المصدر. يُرجى الحصول على الأذونات اللازمة عند إعادة إنتاج المواد أو توزيعها. [بيانات الاستشهاد](original-2016/CITATION.cff)
