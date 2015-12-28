# -*- coding: utf-8 -*-
import os
import codecs
import sys
reload(sys)
sys.setdefaultencoding('utf-8')


pyPath = unicode(os.path.abspath(os.path.dirname(__file__)), "utf-8")
resourcePath = os.path.join(pyPath, u"Assets", u"Resources")
spritePath = os.path.join(resourcePath, u"Sprites", u"Unit")
paths = [os.path.join(resourcePath, u"Sprites", u"Unit"), os.path.join(resourcePath, u"Sprites", u"Agent")]

f = codecs.open(os.path.join(resourcePath, u"preloadsprites.txt"), 'w', "utf-8") 

for p in paths:
	for dirname, dirnames, filenames in os.walk(p):
		print "current dirname >> " + dirname.replace(resourcePath, '')
		for filename in filenames:
			currentFileName = os.path.join(dirname, os.path.basename(filename));

			if filename.lower().endswith((u'.png', u'.jpg', u'.jpeg')):
				imagePath = currentFileName.replace(os.path.join(resourcePath, ''), '')
				resourceName = os.path.splitext(imagePath.replace('\\', '/'))[0]
				print(resourceName)
				f.write(resourceName+u'\n')


f.close()

os.system("Pause")