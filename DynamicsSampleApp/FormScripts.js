function openQC(primaryControl) {
    var formContext = primaryControl;
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "cr908_cartransferreport";
    entityFormOptions["useQuickCreateForm"] = true;

    var date = new Date();

    var carID = formContext.getAttribute("cr908_rentcar").getValue();
    var Id = carID[0].id;
    var Name = carID[0].name;;
    var EntityType = carID[0].entityType;

    var lookup = [];
    lookup[0] = {};
    lookup[0].id = Id;
    lookup[0].name = Name;
    lookup[0].entityType = EntityType;

    var formParameters = {};
    formParameters["cr908_type"] = true;
    formParameters["cr908_reportdate"] = date;
    formParameters["cr908_cartoreport"] = lookup;

    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function success(result) {
            console.log("Record created with ID: " + result.savedEntityReference[0].id +
                " Name: " + result.savedEntityReference[0].name)

            var reportLookup = [];
            reportLookup[0] = {};
            reportLookup[0].id = result.savedEntityReference[0].id;
            reportLookup[0].name = Name;
            reportLookup[0].entityType = result.savedEntityReference[0].entityType;
            formContext.getAttribute("cr908_pickupreport").setValue(reportLookup);

            var reportDate = new Date();
            formContext.getAttribute("cr908_actualpickup").setValue(reportDate);


        },
        function (error) {
            console.log(error);
        });

}

statusOnChange = function (executionContext) {
    var formContext = executionContext.getFormContext();
    var statusOptionSet = formContext.getControl("cr908_status");

    var optionSetAttribute = formContext.getAttribute("cr908_status");

    if (Xrm.Page.ui.getFormType() == 1) {
        var pickList = Xrm.Page.getControl("cr908_status");
        if (optionSetAttribute == null) {
            statusOptionSet.removeOption(575430003);
            statusOptionSet.removeOption(575430004);
        }

        var value = optionSetAttribute.getValue();
        if (value = 575430000) {
            statusOptionSet.removeOption(575430000);
            statusOptionSet.removeOption(575430003);
        }

        if (value = 575430001) {
            statusOptionSet.removeOption(575430000);
            statusOptionSet.removeOption(575430001);
            statusOptionSet.removeOption(575430003);
        }
        if (value = 575430002) {
            statusOptionSet.removeOption(575430000);
            statusOptionSet.removeOption(575430001);
            statusOptionSet.removeOption(575430002);
            statusOptionSet.removeOption(575430004);
        }

    }
}