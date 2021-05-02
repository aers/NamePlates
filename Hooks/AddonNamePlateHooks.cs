using System;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace NamePlates.Hooks
{
    unsafe class AddonNamePlateHooks : IDisposable
    {
        private Plugin _p;

        // hooks
        private delegate void AddonNameplateOnUpdatePrototype(AddonNamePlate* thisPtr, NumberArrayData** numberData,
            StringArrayData** stringData);
        private Hook<AddonNameplateOnUpdatePrototype> hookAddonNameplateOnUpdate;

        // used funcs
        private delegate void AtkTextNodeToggleFixedFontResolutionPrototype(AtkTextNode* thisPtr, byte enable);

        private AtkTextNodeToggleFixedFontResolutionPrototype atkTextNodeToggleFixedFontResolutionFunc;

        private delegate void AtkTextNodeToggleFontCachePrototype(AtkTextNode* thisPtr, byte enable);

        private AtkTextNodeToggleFontCachePrototype atkTextNodeToggleFontCacheFunc;

        private delegate void AtkResNodeToggleVisibilityPrototype(AtkResNode* thisPtr, byte enable);

        private AtkResNodeToggleVisibilityPrototype atkResNodeToggleVisibilityFunc;

        private delegate float AtkUnitBaseGetScalePrototype(AtkUnitBase* thisPtr);

        private AtkUnitBaseGetScalePrototype atkUnitBaseGetScaleFunc;

        private delegate float AtkUnitBaseGetGlobalUIScalePrototype(AtkUnitBase* thisPtr);

        private AtkUnitBaseGetGlobalUIScalePrototype atkUnitBaseGetGlobalUIScaleFunc;

        private delegate ushort AtkResNodeGetPriorityPrototype(AtkResNode* thisPtr);

        private AtkResNodeGetPriorityPrototype atkResNodeGetPriorityFunc;

        private delegate void AtkResNodeSetPriorityPrototype(AtkResNode* thisPtr, ushort priority);

        private AtkResNodeSetPriorityPrototype atkResNodeSetPriorityFunc;

        private delegate void AtkResNodeSetScalePrototype(AtkResNode* thisPtr, float scaleX, float scaleY);

        private AtkResNodeSetScalePrototype atkResNodeSetScaleFunc;

        private delegate void AtkResNodeSetPositionShortPrototype(AtkResNode* thisPtr, short x, short y);

        private AtkResNodeSetPositionShortPrototype atkResNodeSetPositionShortFunc;

        private delegate bool AtkResNodeIsUsingDepthBasedDrawPriorityPrototype(AtkResNode* thisPtr);

        private AtkResNodeIsUsingDepthBasedDrawPriorityPrototype atkResNodeIsUsingDepthBasedDrawPriorityFunc;

        private delegate void AtkResNodeSetUseDepthBasedDrawPriorityPrototype(AtkResNode* thisPtr, bool enable);

        private AtkResNodeSetUseDepthBasedDrawPriorityPrototype atkResNodeSetUseDepthBasedDrawPriorityFunc;

        private delegate AtkResNode* AtkUldManagerSearchNodeByIndexPrototype(AtkUldManager* thisPtr, uint index);

        private AtkUldManagerSearchNodeByIndexPrototype atkUldManagerSearchNodeByIndexFunc;

        public AddonNamePlateHooks(Plugin p)
        {
            _p = p;
        }

        public void Initialize()
        {
            // hooks
            var addonNameplateOnUpdateAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("48 8B C4 41 56 48 81 EC ?? ?? ?? ?? 48 89 58 F0");
            hookAddonNameplateOnUpdate = new Hook<AddonNameplateOnUpdatePrototype>(addonNameplateOnUpdateAddress,
                new AddonNameplateOnUpdatePrototype(AddonNameplateOnUpdateDetour), this);
            hookAddonNameplateOnUpdate.Enable();
            
            // used funcs
            var toggleFontResAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8F ?? ?? ?? ?? 33 D2 48 8B 4C 19 ??");
            atkTextNodeToggleFixedFontResolutionFunc = Marshal.GetDelegateForFunctionPointer<AtkTextNodeToggleFixedFontResolutionPrototype>(toggleFontResAddress);

            var toggleFontCacheAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("48 85 C9 74 79 48 89 5C 24 ??");
            atkTextNodeToggleFontCacheFunc =
                Marshal.GetDelegateForFunctionPointer<AtkTextNodeToggleFontCachePrototype>(toggleFontCacheAddress);

            var toggleVisibilityAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? D1 EE");
            atkResNodeToggleVisibilityFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeToggleVisibilityPrototype>(toggleVisibilityAddress);

            var getScaleAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F BF CB 0F 28 F8");
            atkUnitBaseGetScaleFunc =
                Marshal.GetDelegateForFunctionPointer<AtkUnitBaseGetScalePrototype>(getScaleAddress);

            var getGlobalUIScaleAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F BF 45 00");
            atkUnitBaseGetGlobalUIScaleFunc =
                Marshal.GetDelegateForFunctionPointer<AtkUnitBaseGetGlobalUIScalePrototype>(getGlobalUIScaleAddress);

            var getPriorityAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 66 3B C3 74 13");
            atkResNodeGetPriorityFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeGetPriorityPrototype>(getPriorityAddress);

            var setPriorityAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 8D 45 F0");
            atkResNodeSetPriorityFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeSetPriorityPrototype>(setPriorityAddress);

            var setScaleAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 48 8D 7F 38");
            atkResNodeSetScaleFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeSetScalePrototype>(setScaleAddress);

            var setPositionAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 8D 56 B5");
            atkResNodeSetPositionShortFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeSetPositionShortPrototype>(setPositionAddress);

            var getDepthPriorityAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 41 3A C4 74 61");
            atkResNodeIsUsingDepthBasedDrawPriorityFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeIsUsingDepthBasedDrawPriorityPrototype>(
                    getDepthPriorityAddress);

            var setDepthPriorityAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? FF C6 3B F5 72 E5 BA ?? ?? ?? ??");
            atkResNodeSetUseDepthBasedDrawPriorityFunc =
                Marshal.GetDelegateForFunctionPointer<AtkResNodeSetUseDepthBasedDrawPriorityPrototype>(
                    setDepthPriorityAddress);

            var findNodeAddress =
                this._p.pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 48 8B C8 3B DD");
            atkUldManagerSearchNodeByIndexFunc =
                Marshal.GetDelegateForFunctionPointer<AtkUldManagerSearchNodeByIndexPrototype>(findNodeAddress);
        }

        private void AddonNameplateOnUpdateDetour(AddonNamePlate* thisPtr, NumberArrayData** numberData,
            StringArrayData** stringData)
        {
            var numbers = numberData[5];
            var strings = stringData[4];

            SetTextRenderMode(thisPtr, numbers);

            if (numbers->IntArray[1] != 0)
            {
                numbers->IntArray[1] = 0;
                for (int i = 0; i < 50; i++)
                {
                    thisPtr->NamePlateObjectArray[i].NeedsToBeBaked = 1;
                }
            }

            for (int i = 0; i < 50; i++)
            {
                atkResNodeToggleVisibilityFunc((AtkResNode*)thisPtr->NamePlateObjectArray[i].RootNode, 0);
            }

            var doFullUpdate = numbers->IntArray[4];
            bool doFullUpdateChanged = (doFullUpdate != thisPtr->DoFullUpdate);
            thisPtr->DoFullUpdate = doFullUpdate != 0 ? (byte)1 : (byte)0;

            var nameplateScale = (float) numbers->IntArray[2]; // this is from "Display Name Size" in nameplate config
            var addonScale = atkUnitBaseGetScaleFunc((AtkUnitBase*) thisPtr); // scale of addon itself
            var globalUIScale = atkUnitBaseGetGlobalUIScaleFunc((AtkUnitBase*) thisPtr); // global UI scale in system config

            var finalScale = (float) ((nameplateScale * addonScale * globalUIScale) / 100.0); // nameplateScale is 100, 120, 140 instead of 1.0/1.2/1.4 so divide by 100

            var activeNameplateCount = numbers->IntArray[0];

            if (activeNameplateCount > 0)
            {
                bool hasPriorityChange = false;

                for (int i = 0; i < activeNameplateCount; i++)
                {
                    var numberArrayIndex = i * 19 + 5;
                    var npObjIndex = numbers->IntArray[numberArrayIndex + 15];
                    var npObj = thisPtr->NamePlateObjectArray[npObjIndex];

                    npObj.Priority = activeNameplateCount - i - 1;
                    atkResNodeToggleVisibilityFunc((AtkResNode*) npObj.RootNode, 1);

                    if (atkResNodeGetPriorityFunc((AtkResNode*) npObj.RootNode) != npObj.Priority)
                    {
                        hasPriorityChange = true;
                        atkResNodeSetPriorityFunc((AtkResNode*) npObj.RootNode, (ushort)npObj.Priority);
                    }

                    var nameplateKind = numbers->IntArray[numberArrayIndex + 1];
                    npObj.NameplateKind = (byte) nameplateKind;

                    var x = numbers->IntArray[numberArrayIndex + 3];
                    var y = numbers->IntArray[numberArrayIndex + 4];
                    var depth = numbers->IntArray[numberArrayIndex + 5];
                    var scale = numbers->IntArray[numberArrayIndex + 6];
                    SetCommon(&npObj, x, y, depth, scale, finalScale);

                    var flags = numbers->IntArray[numberArrayIndex + 17];

                    var useDepthBasedDrawPriority = (flags & 0x8) != 0;

                    if (useDepthBasedDrawPriority != atkResNodeIsUsingDepthBasedDrawPriorityFunc((AtkResNode*)npObj.RootNode))
                    {
                        atkResNodeSetUseDepthBasedDrawPriorityFunc((AtkResNode*) npObj.RootNode, useDepthBasedDrawPriority);

                        var rootComponent = npObj.RootNode->Component;

                        if (rootComponent->UldManager.Objects != null)
                        {
                            var count = rootComponent->UldManager.Objects->NodeCount +
                                        rootComponent->UldManager.UnknownCount; // not sure what unknown count is here

                            for (uint nodeIndex = 0; nodeIndex < count; nodeIndex++)
                            {
                                var node = atkUldManagerSearchNodeByIndexFunc(&rootComponent->UldManager, nodeIndex);
                                atkResNodeSetUseDepthBasedDrawPriorityFunc(node, useDepthBasedDrawPriority);
                            }
                        }
                    }

                    bool disableAlternatePartId = (flags & 0x100) != 0;

                    if (disableAlternatePartId)
                    {
                        npObj.ImageNode5->PartId = 1;
                    }
                    else
                    {
                        npObj.ImageNode5->PartId = thisPtr->AlternatePartId;
                    }

                    byte updatesRequestedState = (byte)numbers->IntArray[numberArrayIndex + 2];
                }
            }
        }

        private void SetTextRenderMode(AddonNamePlate* thisPtr, NumberArrayData* numbers)
        {
            var disableFixedFontResolution = numbers->IntArray[3];
            if (disableFixedFontResolution != thisPtr->BakePlate.DisableFixedFontResolution)
            {
                if (disableFixedFontResolution != 0)
                {
                    thisPtr->BakePlate.DisableFixedFontResolution = 1;
                    for (int i = 0; i < 50; i++)
                    {
                        var npObj = thisPtr->NamePlateObjectArray[i];
                        atkTextNodeToggleFixedFontResolutionFunc(npObj.NameText, 0);
                        atkTextNodeToggleFontCacheFunc(npObj.NameText, 0);
                    }
                }
                else
                {
                    thisPtr->BakePlate.DisableFixedFontResolution = 0;
                    for (int i = 0; i < 50; i++)
                    {
                        var npObj = thisPtr->NamePlateObjectArray[i];
                        atkTextNodeToggleFixedFontResolutionFunc(npObj.NameText, 1);
                        atkTextNodeToggleFontCacheFunc(npObj.NameText, 1);
                    }
                    numbers->SetValue(1, 1); // force bake
                }
            }
        }

        private void SetCommon(AddonNamePlate.NamePlateObject* npObj, int x, int y, int depth, int scale, float parentScale)
        {
            var rootNode = npObj->RootNode;

            if (rootNode != null)
            {
                rootNode->AtkResNode.Color.A = (byte) (scale * 0.01 * 255.0);
                var calcScale = (float) Math.Max(scale * 0.01, 0.05) * parentScale;
                atkResNodeSetScaleFunc((AtkResNode*) rootNode, calcScale, calcScale);
                npObj->BakeData.Alpha = (byte) Math.Sqrt(Math.Sqrt(scale * 0.05));
                var newX = (short) (x - rootNode->AtkResNode.Width / 2);
                var newY = (short) (y - rootNode->AtkResNode.Height);
                atkResNodeSetPositionShortFunc((AtkResNode*) rootNode, newX, newY);
                rootNode->AtkResNode.Depth = *(float*) &depth;
            }
        }

        public void Dispose()
        {
            hookAddonNameplateOnUpdate.Disable();
            hookAddonNameplateOnUpdate.Dispose();
        }
    }
}
