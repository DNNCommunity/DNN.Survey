﻿function CheckBoxValidatorEvaluateIsValid(val) {
   var control = document.getElementById(val.controltovalidate);
   return control.checked == Boolean(val.mustBeChecked.toLowerCase() == "true");
}

function CheckBoxListValidatorEvaluateIsValid(val) {
   var control = document.getElementById(val.controltovalidate);

   var selectedItemCount = 0;
   var liIndex = 0;
   var currentListItem = document.getElementById(control.id + '_' + liIndex.toString());
   while (currentListItem != null) {
      if (currentListItem.checked) selectedItemCount++;
      liIndex++;
      currentListItem = document.getElementById(control.id + '_' + liIndex.toString());
   }

   return selectedItemCount >= parseInt(val.minimumNumberOfSelectedCheckBoxes);
}

