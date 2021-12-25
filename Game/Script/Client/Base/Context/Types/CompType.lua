local M = {
    ["Component"] = GComponent,
    ["ScrollPane"] = GScrollPane,
    ["Label"] = GLabel,
    ["RichText"] = GRichText,
    ["Button"] = GButton,
    ["Tag"] = GTag,
    ["List"] = GList,
    ["Checkbox"] = GCheckBox,
    ["RadioButton"] = GRadioButton,
    ["TextInput"] = GTextInput,
    ["Slider"] = GSlider,
    ["SliderInt"] = GSliderInt,
    ["ComboBox"] = GComboBox,
    ["ProgressBar"] = GProgressBar,
    ["Image"] = GImage,
    ["ScrollView"] = GScrollView,
    ["Loader"] = GLoader,
    ["Graph"] = GGraph,
    ["Accordion"] = GAccordion,
}


rawset(_G, "CompType", false)
CompType = M