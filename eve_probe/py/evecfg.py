# uncompyle6 version 2.11.1
# Python bytecode 2.7 (62211)
# Decompiled from: Python 2.7 (r27:82525, Jul  4 2010, 09:01:59) [MSC v.1500 32 bit (Intel)]
# Embedded file name: e:\jenkins\workspace\client_TRANQUILITY\branches\release\TRANQUILITY\eve\common\script\sys\eveCfg.py
# Compiled at: 2017-07-06 15:18:13
import math
import sys
import types
import random
import copy
import re
import sqlite3
import collections
import blue
import bluepy
#from crimewatch.util import GetKillReportHashValue
import eve.common.script.util.utillib_bootstrap as utillib
#import carbon.common.script.util.format as formatUtil
#import eve.common.script.util.eveFormat as evefmtutil
from carbon.common.script.sys.crowset import CRowset
import carbon.common.script.sys.service as service
import carbon.common.script.net.machobase as machobase
import localization
import re
import sqlite3
import industry
import remotefilecache
import uthread
#import carbon.common.script.sys.cfg as sysCfg
#from evegraphics.fsd.graphicIDs import GetGraphicFile
import pytelemetry.zoning as telemetry
import collections
import evetypes
import fsdlite
from eveprefs import prefs, boot
from inventorycommon.util import IsSubsystemFlagVisible
globals().update(service.consts)
import const
import standingUtil
import fsd.schemas.binaryLoader as fsdBinaryLoader
import spacecomponents.common.factory
from spacecomponents.common.helper import HasCargoBayComponent
import evewar.util
from eve.common.script.sys.idCheckers import IsCelestial, IsConstellation, IsRegion, IsSolarSystem, IsStation, IsNPCCorporation, IsNewbieSystem
import eve.common.script.sys.idCheckers as idCheckers
OWNER_AURA_IDENTIFIER = -1
OWNER_SYSTEM_IDENTIFIER = -2

class Standings():
    __guid__ = 'eveCfg.Standings'
    __passbyvalue__ = 1

    def __init__(self, fromID, fromFactionID, fromCorpID, fromCharID, toID, toFactionID, toCorpID, toCharID):
        self.fromID, self.fromFactionID, self.fromCorpID, self.fromCharID, self.toID, self.toFactionID, self.toCorpID, self.toCharID = (
         fromID, fromFactionID, fromCorpID, fromCharID, toID, toFactionID, toCorpID, toCharID)
        self.faction = utillib.KeyVal(faction=0.0, corp=0.0, char=0.0)
        self.corp = utillib.KeyVal(faction=0.0, corp=0.0, char=0.0)
        self.char = utillib.KeyVal(faction=0.0, corp=0.0, char=0.0)

    def __str__(self):
        return 'Standing from %s toward %s: faction:(%s,%s,%s), corp:(%s,%s,%s), char:(%s,%s,%s)' % (
         self.fromID, self.toID,
         self.faction.faction, self.faction.corp, self.faction.char,
         self.corp.faction, self.corp.corp, self.corp.char,
         self.char.faction, self.char.corp, self.char.char)

    def __repr__(self):
        return self.__str__()

    def CanUseAgent(self, level, agentTypeID=None, noL1Check=1):
        return CanUseAgent(level, agentTypeID, self.faction.char, self.corp.char, self.char.char, self.fromCorpID, self.fromFactionID, {}, noL1Check)

    def __getattr__(self, theKey):
        if theKey == 'minimum':
            m = None
            for each in (self.faction, self.corp, self.char):
                for other in (each.faction, each.corp, each.char):
                    if other != 0.0 and (m is None or other < m):
                        m = other

            if m is None:
                return 0.0
            return m
        else:
            if theKey == 'maximum':
                m = None
                for each in (self.faction, self.corp, self.char):
                    for other in (each.faction, each.corp, each.char):
                        if other != 0.0 and (m is None or other > m):
                            m = other

                if m is None:
                    return 0.0
                return m
            if theKey == 'direct':
                if self.fromID == self.fromFactionID:
                    tmp = self.faction
                elif self.fromID == self.fromCorpID:
                    tmp = self.corp
                elif self.fromID == self.fromCharID:
                    tmp = self.char
                if self.toID == self.toFactionID:
                    return tmp.faction
                else:
                    if self.toID == self.toCorpID:
                        return tmp.corp
                    if self.toID == self.toCharID:
                        return tmp.char
                    return 0.0

            else:
                if theKey == 'all':
                    return [
                     (
                      self.fromFactionID, self.toFactionID, self.faction.faction),
                     (
                      self.fromFactionID, self.toCorpID, self.faction.corp),
                     (
                      self.fromFactionID, self.toCharID, self.faction.char),
                     (
                      self.fromCorpID, self.toFactionID, self.corp.faction),
                     (
                      self.fromCorpID, self.toCorpID, self.corp.corp),
                     (
                      self.fromCorpID, self.toCharID, self.corp.char),
                     (
                      self.fromCharID, self.toFactionID, self.char.faction),
                     (
                      self.fromCharID, self.toCorpID, self.char.corp),
                     (
                      self.fromCharID, self.toCharID, self.char.char)]
                raise AttributeError(theKey)
            return


def CanUseAgent(level, agentTypeID, fac, coc, cac, fromCorpID, fromFactionID, skills, noL1Check=1):
    if agentTypeID == const.agentTypeAura:
        return True
    else:
        if level == 1 and agentTypeID != const.agentTypeResearchAgent and noL1Check:
            return 1
        m = (level - 1) * 2.0 - 1.0
        if boot.role == 'client':
            if not skills:
                mySkills = sm.GetService('skills').GetSkill
                for skillTypeID in (
                 const.typeConnections,
                 const.typeDiplomacy,
                 const.typeCriminalConnections):
                    skillInfo = mySkills(skillTypeID)
                    if skillInfo:
                        skills[skillTypeID] = skillInfo

            unused, facBonus = standingUtil.GetStandingBonus(fac, fromFactionID, skills)
            unused, cocBonus = standingUtil.GetStandingBonus(coc, fromFactionID, skills)
            unused, cacBonus = standingUtil.GetStandingBonus(cac, fromFactionID, skills)
            if facBonus > 0.0:
                fac = (1.0 - (1.0 - fac / 10.0) * (1.0 - facBonus / 10.0)) * 10.0
            if cocBonus > 0.0:
                coc = (1.0 - (1.0 - coc / 10.0) * (1.0 - cocBonus / 10.0)) * 10.0
            if cacBonus > 0.0:
                cac = (1.0 - (1.0 - cac / 10.0) * (1.0 - cacBonus / 10.0)) * 10.0
        if max(fac, coc, cac) >= m and min(fac, coc, cac) > -2.0:
            if agentTypeID == const.agentTypeResearchAgent and coc < m - 2.0:
                return 0
            return 1
        return 0


def GetStrippedEnglishMessage(messageID):
    msg = localization._GetRawByMessageID(messageID, 'en-us')
    if msg:
        regex = '</localized>|<localized>|<localized .*?>|<localized *=.*?>'
        return ''.join(re.split(regex, msg))
    else:
        return ''


def CfgFsdDustIcons():
    return cfg.fsdDustIcons


def CfgIcons():
    return cfg.icons


def CfgSounds():
    return cfg.sounds


def CfgInvcontrabandFactionsByType():
    return cfg.invcontrabandFactionsByType


def CfgShiptypes():
    return cfg.shiptypes


def CfgAverageMarketPrice():
    return cfg._averageMarketPrice


def StackSize(item):
    if item[const.ixQuantity] < 0:
        return 1
    return item[const.ixQuantity]


def Singleton(item):
    if item[const.ixQuantity] < 0:
        return -item[const.ixQuantity]
    if 30000000 <= item[const.ixLocationID] < 40000000:
        return 1
    return 0


def RamActivityVirtualColumn(row):
    return cfg.ramaltypes.Get(row.assemblyLineTypeID).activityID


def IsSystem(ownerID):
    return idCheckers.IsSystem(ownerID)


def IsDustType(typeID):
    return idCheckers.IsDustType(typeID)


def IsNPCCharacter(ownerID):
    return idCheckers.IsNPCCharacter(ownerID)


def IsSystemOrNPC(ownerID):
    return idCheckers.IsSystemOrNPC(ownerID)


def IsFaction(ownerID):
    return idCheckers.IsFaction(ownerID)


def IsCorporation(ownerID):
    return idCheckers.IsCorporation(ownerID)


def IsCharacter(ownerID):
    return idCheckers.IsCharacter(ownerID)


def CheckShipHasFighterBay(shipID):
    item = sm.GetService('godma').GetItem(shipID)
    if not item:
        return False
    godmaSM = sm.GetService('godma').GetStateManager()
    return godmaSM.GetType(item.typeID).fighterCapacity > 0


def IsPlayerAvatar(itemID):
    return idCheckers.IsCharacter(itemID)


def IsOwner(ownerID):
    return idCheckers.IsOwner(ownerID)


def IsAlliance(ownerID):
    return idCheckers.IsAlliance(ownerID)


def IsUniverseCelestial(itemID):
    return idCheckers.IsUniverseCelestial(itemID)


def IsDistrict(itemID):
    return idCheckers.IsDistrict(itemID)


def IsStargate(itemID):
    return idCheckers.IsStargate(itemID)


def IsWorldSpace(itemID):
    return idCheckers.IsWorldSpace(itemID)


def IsOutpost(itemID):
    return idCheckers.IsOutpost(itemID)


def IsTrading(itemID):
    return idCheckers.IsTrading(itemID)


def IsOfficeFolder(itemID):
    return idCheckers.IsOfficeFolder(itemID)


def IsFactoryFolder(itemID):
    return idCheckers.IsFactoryFolder(itemID)


def IsUniverseAsteroid(itemID):
    return idCheckers.IsUniverseAsteroid(itemID)


def IsJunkLocation(locationID):
    return idCheckers.IsJunkLocation(locationID)


def IsControlBunker(itemID):
    return idCheckers.IsControlBunker(itemID)


def IsFakeItem(itemID):
    return idCheckers.IsFakeItem(itemID)


def IsStarbase(categoryID):
    return idCheckers.IsStarbase(categoryID)


def IsPreviewable(typeID):
    if not evetypes.Exists(typeID):
        return False
    else:
        if IsApparel(typeID):
            return True
        if IsShipSkin(typeID):
            return True
        if evetypes.GetGraphicID(typeID) is None:
            return False
        return evetypes.GetCategoryID(typeID) in const.previewCategories or evetypes.GetGroupID(typeID) in const.previewGroups


def IsShip(typeID):
    if not evetypes.Exists(typeID):
        return False
    return evetypes.GetCategoryID(typeID) == const.categoryShip


def IsShipSkin(typeID):
    if not evetypes.Exists(typeID):
        return False
    return evetypes.GetGroupID(typeID) == const.groupShipSkins


def IsApparel(typeID):
    if not evetypes.Exists(typeID):
        return False
    return evetypes.GetCategoryID(typeID) == const.categoryApparel


def IsBlueprint(typeID):
    if not evetypes.Exists(typeID):
        return False
    return evetypes.GetCategoryID(typeID) == const.categoryBlueprint


def IsPlaceable(typeID):
    if not evetypes.Exists(typeID):
        return False
    return evetypes.GetCategoryID(typeID) == const.categoryPlaceables


def IsEveUser(userID):
    return idCheckers.IsEveUser(userID)


def IsDustUser(userID):
    return idCheckers.IsDustUser(userID)


def IsDustCharacter(characterID):
    return idCheckers.IsDustCharacter(characterID)


def IsEvePlayerCharacter(ownerID):
    return idCheckers.IsEvePlayerCharacter(ownerID)


def IsPlayerOwner(ownerID):
    return idCheckers.IsPlayerOwner(ownerID)


def GetCharacterType(characterID):
    if IsEveUser(characterID):
        return 'capsuleer'
    else:
        if IsDustCharacter(characterID):
            return 'mercenary'
        return 'unknown'


def IsOutlawStatus(securityStatus):
    if securityStatus is None:
        return False
    else:
        return round(securityStatus, 1) <= const.outlawSecurityStatus
        return


OWNER_NAME_OVERRIDES = {OWNER_AURA_IDENTIFIER: 'UI/Agents/AuraAgentName',OWNER_SYSTEM_IDENTIFIER: 'UI/Chat/ChatEngine/EveSystem'
   }


class PropertyBag():

    def __init__(self):
        self.Reset()

    def LoadFromMoniker(self, moniker_dict):
        import base64
        import cPickle
        self.__dict__['bag'] = cPickle.loads(base64.decodestring(moniker_dict))

    def GetMoniker(self):
        import base64
        import cPickle
        tupl = (
         self.__guid__, base64.encodestring(cPickle.dumps(self.__dict__['bag'])))
        return base64.encodestring(cPickle.dumps(tupl, 1)).rstrip()

    def AddProperty(self, propertyName, propertyValue):
        self.__dict__['bag'][propertyName] = propertyValue

    def HasProperty(self, propertyName):
        return self.__dict__['bag'].has_key(propertyName)

    def GetProperty(self, propertyName):
        if self.__dict__['bag'].has_key(propertyName):
            return self.__dict__['bag'][propertyName]

    def RemoveProperty(self, propertyName):
        if self.__dict__['bag'].has_key(propertyName):
            del self.__dict__['bag'][propertyName]

    def GetProperties(self):
        return self.__dict__['bag'].items()

    def Reset(self):
        self.__dict__['bag'] = {}


def _LoadMessagesFromFSD():
    return fsdBinaryLoader.LoadFSDDataForCFG('res:/staticdata/dialogs.static', 'res:/staticdata/dialogs.schema', optimize=False)


def IsWarInHostileState(row):
    return evewar.util.IsWarInHostileState(row, blue.os.GetWallclockTime())


def IsWarActive(row):
    if row.timeFinished is None or blue.os.GetWallclockTime() < row.timeFinished:
        return 1
    else:
        return 0


def IsAllyActive(row, time=None):
    if time is None:
        time = blue.os.GetWallclockTime()
    return row.timeStarted < time < row.timeFinished


def IsAtWar(wars, entities):
    return evewar.util.IsAtWar(wars, entities, blue.os.GetWallclockTime())


def IsPolarisFrigate(typeID):
    return typeID in (
     const.typePolarisCenturion,
     const.typePolarisCenturionFrigate,
     const.typePolarisInspectorFrigate,
     const.typePolarisLegatusFrigate,
     const.typePolarisEnigmaFrigate)


def GetReprocessingOptions(types):
    options = []
    optionTypes = {}
    noneTypes = [
     const.typeCredits, const.typeBookmark, const.typeBiomass]
    noneGroups = [const.groupRookieship, const.groupMineral]
    noneCategories = [const.categoryBlueprint, const.categoryReaction]
    for key in types.iterkeys():
        typeID = key
        isRecyclable = 0
        isRefinable = 0
        groupID = evetypes.GetGroupID(typeID)
        categoryID = evetypes.GetCategoryID(typeID)
        if typeID not in noneTypes and groupID not in noneGroups and categoryID not in noneCategories:
            if typeID in cfg.invtypematerials:
                materials = cfg.invtypematerials[typeID]
                if len(materials) > 0:
                    if categoryID == const.categoryAsteroid or groupID == const.groupHarvestableCloud:
                        isRefinable = 1
                    else:
                        isRecyclable = 1
        options.append(utillib.KeyVal(typeID=typeID, isRecyclable=isRecyclable, isRefinable=isRefinable))

    for option in options:
        optionTypes[option.typeID] = option

    return optionTypes


def MakeConstantName(val, prefix):
    name = val.replace(' ', '')
    if name == '':
        name = 'invalidName_' + val
    name = prefix + name[0].upper() + name[1:]
    ret = ''
    okey = range(ord('a'), ord('z') + 1) + range(ord('A'), ord('Z') + 1) + range(ord('0'), ord('9') + 1)
    for ch in name:
        if ord(ch) in okey:
            ret += ch

    if ret == '':
        ret = 'invalidName_' + ret
    elif ord(ret[0]) in range(ord('0'), ord('9') + 1):
        ret = '_' + ret
    return ret


locationPathByFlagID = {const.flagCargo: 'UI/Ship/CargoHold',
   const.flagDroneBay: 'UI/Ship/DroneBay',
   const.flagShipHangar: 'UI/Ship/ShipMaintenanceBay',
   const.flagSpecializedFuelBay: 'UI/Ship/FuelBay',
   const.flagSpecializedOreHold: 'UI/Ship/OreHold',
   const.flagSpecializedGasHold: 'UI/Ship/GasHold',
   const.flagSpecializedMineralHold: 'UI/Ship/MineralHold',
   const.flagSpecializedSalvageHold: 'UI/Ship/SalvageHold',
   const.flagSpecializedShipHold: 'UI/Ship/ShipHold',
   const.flagSpecializedSmallShipHold: 'UI/Ship/SmallShipHold',
   const.flagSpecializedMediumShipHold: 'UI/Ship/MediumShipHold',
   const.flagSpecializedLargeShipHold: 'UI/Ship/LargeShipHold',
   const.flagSpecializedIndustrialShipHold: 'UI/Ship/IndustrialShipHold',
   const.flagSpecializedAmmoHold: 'UI/Ship/AmmoHold',
   const.flagSpecializedCommandCenterHold: 'UI/Ship/CommandCenterHold',
   const.flagSpecializedPlanetaryCommoditiesHold: 'UI/Ship/PlanetaryCommoditiesHold',
   const.flagSpecializedMaterialBay: 'UI/Ship/MaterialBay',
   const.flagFighterBay: 'UI/Ship/FighterBay'
   }

def GetShipFlagLocationName(flagID):
    if flagID in const.hiSlotFlags:
        locationPath = 'UI/Ship/HighSlot'
    elif flagID in const.medSlotFlags:
        locationPath = 'UI/Ship/MediumSlot'
    elif flagID in const.loSlotFlags:
        locationPath = 'UI/Ship/LowSlot'
    elif flagID in const.rigSlotFlags:
        locationPath = 'UI/Ship/RigSlot'
    elif IsSubsystemFlagVisible(flagID):
        locationPath = 'UI/Ship/Subsystem'
    elif flagID in const.flagCorpSAGs:
        locationPath = 'UI/Corporations/Common/CorporateHangar'
    elif flagID in const.fighterTubeFlags:
        locationPath = 'UI/Ship/FighterLaunchTube'
    else:
        locationPath = locationPathByFlagID.get(flagID, '')
    if locationPath:
        return localization.GetByLabel(locationPath)
    else:
        return ''


def GetSunWarpInPoint(ballID, position, radius):
    offset = 100000
    x = position[0] + (radius + offset) * math.cos(radius)
    y = position[1] + radius / 5
    z = position[2] - (radius + offset) * math.sin(radius)
    return (
     x, y, z)


def GetPlanetWarpInPoint(ballID, position, radius):
    dx = float(position[0])
    dz = float(-position[2])
    f = float(dz) / float(math.sqrt(dx ** 2 + dz ** 2))
    if dz > 0 and dx > 0 or dz < 0 and dx > 0:
        f *= -1.0
    theta = math.asin(f)
    myRandom = random.Random(ballID)
    rr = (myRandom.random() - 1.0) / 3.0
    theta += rr
    offset = 1000000
    FACTOR = 20.0
    dd = math.pow((FACTOR - 5.0 * math.log10(radius / 1000000) - 0.5) / FACTOR, FACTOR) * FACTOR
    dd = min(10.0, max(0.0, dd))
    dd += 0.5
    offset += radius * dd
    d = radius + offset
    x = 1000000
    z = 0
    x = position[0] + math.sin(theta) * d
    y = position[1] + radius * math.sin(rr) * 0.5
    z = position[2] - math.cos(theta) * d
    return (
     x, y, z)


def GetWarpInPoint(ballID, position, radius):
    offset = 5000000
    p = const.jumpRadiusFactor / 100.0
    x = position[0] + (radius + offset) * math.cos(radius)
    y = position[1] + p * radius - 7500.0
    z = position[2] - (radius + offset) * math.sin(radius)
    return (
     x, y, z)


def IconFile(iconID):
    try:
        return cfg.icons.Get(iconID).iconFile
    except Exception:
        return ''


def GetActiveShip():
    return session.shipid


def InSpace():
    return bool(session.solarsystemid) and bool(session.shipid) and session.structureid in (session.shipid, None)


def InShip():
    return bool(session.shipid) and bool(session.shipid != session.structureid)


def InShipInSpace():
    return bool(session.solarsystemid) and bool(session.shipid) and not bool(session.structureid)


def IsDocked():
    return bool(session.stationid2) or IsDockedInStructure()


def InStructure():
    return bool(session.structureid)


def IsDockedInStructure():
    return bool(session.structureid) and bool(session.structureid != session.shipid)


def IsControllingStructure():
    return bool(session.structureid) and bool(session.structureid == session.shipid)


def IsBookmarkModerator(corpRole):
    return corpRole & const.corpRoleChatManager == const.corpRoleChatManager


BULKVERSION = 3
BULKDEFINITIONS = {'typeSkillReqs': {'source': 'inventory.typesBySkillLevelVx','keys': [
                            (
                             'skillTypeID', 'integer'),
                            (
                             'skillLevel', 'integer'),
                            (
                             'typeID', 'integer'),
                            (
                             'marketGroupID', 'integer'),
                            (
                             'marketGroupNameID', 'integer'),
                            (
                             'metaLevel', 'integer')],
                     'indexes': [
                               [
                                'skillTypeID']]
                     },
   'marketGroups': {'source': 'inventory.marketGroups','keys': [
                           (
                            'marketGroupID', 'integer PRIMARY KEY'),
                           (
                            'parentGroupID', 'integer'),
                           (
                            'marketGroupNameID', 'integer'),
                           (
                            'descriptionID', 'integer'),
                           (
                            'iconID', 'integer')],
                    'indexes': [
                              [
                               'parentGroupID']]
                    }
   }
bulkDataTableDefinitions = BULKDEFINITIONS
bulkDataVersion = BULKVERSION