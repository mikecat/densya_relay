TARGET=DensyaRelay.exe
OPTIONS= \
	/target:winexe \
	/optimize+ \
	/warn:4 \
	/codepage:65001

SOURCES= \
	AssemblyInfo.cs \
	DensyaRelay.cs \
	AdvancedConfiguration.cs \
	SendKeyWindow.cs \
	ControlUtils.cs \
	RegistryIO.cs \
	UItext.cs \
	JapaneseUIText.cs \
	EnglishUIText.cs

$(TARGET): $(SOURCES)
	csc /out:$@ $(OPTIONS) $^
