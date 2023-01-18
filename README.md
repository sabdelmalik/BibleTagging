# Bible Tagging
This project was started to specifically tag Smith & Van Dyck Arabic Bible translation with Strong’s numbers. However, the process used can be applied to virtually any Bible. It was recently applied to a Zokam (Burmese) Bible.
# Process Summary
1.	The Bible text is provided in a Verse Per Line  (VPL) format.
2.	The text it then normalized and [stemmed](https://www.geeksforgeeks.org/snowball-stemmer-nlp/). This basically means removing any diacritics and punctuation marks, then reducing words to a basic form (it is not exactly the roots of the words). 
3.	Using a publicly available software, [Berkely Aligner](https://github.com/mhajiloo/berkeleyaligner), to map the text Strong’s number. The process is not perfect, but has around 70% accuracy (which can vary between languages). 
4.	As a last step, the mapped text is converted to [OSIS](http://crosswire.org/osis/) format from which a [SWORD module](https://www.crosswire.org/sword/develop/swordmodule/) can be created. SWORD modules are used by many Bible study tools.

Refer to **./docs/From-VPL-Bible-to-tagged-SWORD-module.docx** in this repository for details.
Refer to [From-VPL-Bible-to-tagged-SWORD-module](./docs/From-VPL-Bible-to-tagged-SWORD-module.docx).

