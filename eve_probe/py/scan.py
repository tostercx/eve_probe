import state
import re
import random

stepSize = [0.0, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0, 16.0, 32.0, 64.0, 128.0]

scanning = False
target = None
curStep = 6
scanMgrOid = None

liveProbes = {}
signatures = {}

scanMgrCall = 103461L

formation = [
    [0, 0, 0],
    [0, 74798940160, 0],
    [0, -74798923776, 0],
    [74798923776, 0, 0],
    [23114153984, 0, 71138011904],
    [-60513599488, 0, 43965709056],
    [-60513599488, 0, -43965711616],
    [23114153984, 0, -71138022656]
]

def getScanMgr(node):
    return {'__class__': 'carbon.common.script.net.machoNetPacket.CallReq',
        'applicationID': None,
        'command': 6,
        'compressedPart': 0,
        'contextKey': None,
        'destination': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                        'addressType': 1,
                        'callID': None,
                        'nodeID': long(node),
                        'service': 'scanMgr'},
        'languageID': None,
        'oob': {},
        'payload': (0,
                    {'__class__': 'blue.MarshalStream',
                    '__content__': (1,
                                    'GetSystemScanMgr',
                                    (),
                                    {'machoVersion': 1})}),
        'source': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                'addressType': 2,
                'callID': scanMgrCall,
                'clientID': 0,
                'service': None},
        'userID': state.status['userID']}

def beginScan(node, oid, center, step):
    sRange = stepSize[step]

    probeN = 0
    for probeID in liveProbes:
        pos = tuple([x*sRange + float(y) for x, y in zip(formation[probeN][:], center)])
        liveProbes[probeID]['destination'] = pos
        liveProbes[probeID]['pos'] = pos
        liveProbes[probeID]['rangeStep'] = step
        liveProbes[probeID]['scanRange'] = sRange * 149597870700.0
        liveProbes[probeID]['state'] = 1
        probeN += 1


    return {'__class__': 'carbon.common.script.net.machoNetPacket.CallReq',
        'applicationID': None,
        'command': 6,
        'compressedPart': 0,
        'contextKey': None,
        'destination': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                     'addressType': 1,
                     'callID': None,
                     'nodeID': long(node),
                     'service': None},
        'languageID': None,
        'oob': {},
        'payload': (1,
             {'__class__': 'blue.MarshalStream',
              '__content__': (oid,
                              'RequestScans',
                              (liveProbes,),
                              {'machoVersion': 1})}),
        'source': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                'addressType': 2,
                'callID': 10345L, # mb update this somehow?
                'clientID': 0,
                'service': None},
        'userID': state.status['userID']}


def scanStep():
    global scanning, target, curStep

    if not 'locationid' in state.status:
        return

    if not state.status['locationid'] in state.status['oid']:
        return

    oid = state.status['oid'][state.status['locationid']]
    node = long(re.search('=(\d+):', oid).group(1))

    #if len(state.status['nodesOfInterest']) == 0:
    #    return
    #node = state.status['nodesOfInterest'][0]

    if scanMgrOid == None:
        scanning = True
        state.injectQueue.append(getScanMgr(node))
        return

    if curStep < 1:
        if target in signatures:
            del signatures[target]
            target = None
            if len(signatures) == 0:
                return
    
    scanning = True

    if target == None:
        target = signatures.values()[0]['targetID']
        curStep = 6
    
    state.injectQueue.append(beginScan(node, scanMgrOid, signatures[target]['position'], curStep))
    curStep -= 1



def step(pck):
    global signatures, scanning, scanMgrOid, target, curStep

    #state.status['liveProbes'] = liveProbes
    state.status['signatures'] = signatures
    state.status['scanMgrOid'] = scanMgrOid
    

    if '__class__' in pck:
        if pck['__class__'].endswith('.CallRsp'):
            if 'callID' in pck['destination'] and pck['destination']['callID'] == scanMgrCall:
                scanMgrOid = pck['oob']['OID+'].keys()[0]
                scanning = False

        # check notifications
        elif pck['__class__'].endswith('.Notification'):
            if 'broadcastID' in pck['destination']:
                # anomaly reload
                if pck['destination']['broadcastID'] == 'OnSignalTrackerFullState':
                    signatures = {} # reset list
                    scanMgrOid = None
                    tracked = pck['payload'][1]['__content__'][1][1][1][1] # last is type of anomaly
                    for key in tracked:
                        item = tracked[key]
                        if item['__class__'].endswith('.SignatureInfo') and item['archetypeID'] in [38,44,45]: # wormhole, relic, data
                            signatures[item['targetID']] = item
                
                # scan done
                if pck['destination']['broadcastID'] == 'OnSystemScanStopped':
                    scanning = False
                    results = pck['payload'][1]['__content__'][1][1][1]
                    if results != None:
                        for result in results:
                            if result['id'] in signatures:
                                if result['certainty'] == 1.0:
                                    del signatures[result['id']]
                                    if target == result['id']:
                                        target = None
                                elif result['degraded'] == False:
                                    data = result['data']
                                    if type(data) is float:
                                        if type(result['pos']) == tuple:
                                            signatures[result['id']]['position'] = result['pos']
                                        
                                    else:
                                        if len(data) == 2:
                                            #data = tuple([(x + y)/2 for x, y in zip(data[0], data[1])])
                                            data = data[random.randint(0,1)]
                                            #curStep += 1
                                        if type(data) == dict:
                                            data = data['point']
                                            curStep += 1
                                        signatures[result['id']]['position'] = data
                                        signatures[result['id']]['certainty'] = result['certainty']
                
                # new probe
                if pck['destination']['broadcastID'] == 'OnNewProbe':
                    probe = pck['payload'][1]['__content__'][1][1][0]
                    liveProbes[probe['probeID']] = probe
                    if len(liveProbes) > 7:
                        scanning = False
                        target = None
                
                # probe gone
                if pck['destination']['broadcastID'] == 'OnRemoveProbe':
                    probeID = pck['payload'][1]['__content__'][1][1][0]
                    liveProbes.pop(probeID, None)
                    
                    if len(liveProbes) == 0:
                        scanning = False
                        target = None
                        # send reload
    
    
    if not scanning and len(liveProbes) > 7 and len(signatures) > 0:
        scanStep()
