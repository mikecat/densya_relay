TARGET=DensyaRelay.exe
OPTIONS= \
	/target:winexe \
	/optimize+ \
	/warn:4 \
	/codepage:65001

SOURCES= \
	AssemblyInfo.cs \
	DensyaRelay.cs \
	RegistryIO.cs \
	UItext.cs \
	JapaneseUIText.cs \
	EnglishUIText.cs

$(TARGET): $(SOURCES)
	csc /out:$@ $(OPTIONS) $^
