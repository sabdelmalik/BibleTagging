#############################
# The Berkeley Word Aligner #
#############################

The Berkeley Word Aligner is a statistical machine translation tool that automatically aligns words in a sentence-aligned parallel corpus.

-------------
Install & Run
-------------

No installation is required beyond expanding the distribution archive.  

To train the unsupervised aligner on the provided sample data, run the following command:

  bash unsupervised_align example_confs/french.conf

This invocation can also be identically achieved via the command:

  java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.nlp.wordAlignment.Main ++example_confs/french.conf

Similarly, for the supervised aligner, run:
  
  bash supervised_align example_confs/chinese_input_hmm.conf example_confs/chinese_train.conf example_confs/chinese_align.conf

Or, you can run the following 3 commands in sequence:
  java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.wordAlignment.Main ++example_confs/chinese_input_hmm.conf
  java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.wordAlignment.symmetric.itg.ITGMain ++example_confs/chinese_train.conf
  java -server -mx1000m -cp berkeleyaligner.jar edu.berkeley.wordAlignment.symmetric.itg.ITGMain ++example_confs/chinese_align.conf
  
The documentation directory contains further information on configuration options.

-----------
Information
-----------

For more information about the package as a whole, please visit:

  http://nlp.cs.berkeley.edu/pages/wordaligner.html

The source code for this project, along with further information and resources, is available online:

  http://code.google.com/p/berkeleyaligner

Enjoy!

============================================================
(C) Copyright 2007, John DeNero and Percy Liang

http://www.cs.berkeley.edu/~denero
http://www.cs.berkeley.edu/~pliang
http://nlp.cs.berkeley.edu

Permission is granted for anyone to copy, use, or modify these programs and
accompanying documents for purposes of research or education, provided this
copyright notice is retained, and note is made of any changes that have been
made.

These programs and documents are distributed without any warranty, express or
implied.  As the programs were written for research purposes only, they have
not been tested to the degree that would be advisable in any important
application.  All use of these programs is entirely at the user's own risk.
============================================================