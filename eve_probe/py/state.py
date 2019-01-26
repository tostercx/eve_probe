from pprint import pformat
import const


status = {}
callType = {}
bridges = []
injectQueue = []

status['oid'] = {}
status['lastBoundSS'] = None


def on_packet(pck):
    
    # always copy userID
    if 'userID' in pck:
        status['userID'] = pck['userID']
    
    
    if '__class__' in pck:
        
        # requests
        if pck['__class__'].endswith('.CallReq'):
            
            # get payload method name
            methex = ''
            try:
                methex = pck['payload'][1]['__content__'][1]
                if methex.__class__.__name__ != 'str':
                    methex = ''
            except:
                pass
            
            # get callID
            callID = pck.get('source', {}).get('callID', '')
            callID = '' if not callID else str(callID)
            
            if 'destination' in pck:
                if 'service' in pck['destination'] and callID:
                
                    # GetJumpBridgesWithMyAccess
                    if pck['destination']['service'] == 'structureDirectory' and methex == 'GetJumpBridgesWithMyAccess':
                        callType[callID] = methex
                    
                    # SS MachoBindObject
                    elif pck['destination']['service'] == 'beyonce' and methex == 'MachoBindObject':
                        try:
                            status['lastBoundSS'] = pck['payload'][1]['__content__'][2][0]
                            if status['lastBoundSS'] >= const.minSolarSystem and status['lastBoundSS'] <= const.maxSolarSystem:
                                callType[callID] = methex
                            else:
                                status['lastBoundSS'] = None
                        except:
                            pass
        
        
        # responses
        elif pck['__class__'].endswith('.CallRsp'):
            
            # get callID
            callID = pck.get('destination', {}).get('callID', '')
            callID = '' if not callID else str(callID)
            
            if callID and callID in callType:
                
                # GetJumpBridgesWithMyAccess
                if callType[callID] == 'GetJumpBridgesWithMyAccess':
                    try:
                        bridges = pck['payload']['__content__'][0]
                        status['gotBridges'] = True
                    except:
                        pass
                
                # SS MachoBindObject
                elif callType[callID] == 'MachoBindObject' and status['lastBoundSS']:
                    try:
                        status['oid'][status['lastBoundSS']] = pck['oob']['OID+'].keys()[0]
                        status['lastBoundSS'] = None
                    except:
                        pass
                
                # assume we processed it
                del callType[callID]
        
        
        # copy all session changes and prev vals
        elif pck['__class__'].endswith('.SessionChangeNotification'):
            
            try:
                status['nodesOfInterest'] = pck['nodesOfInterest']
            except:
                pass
            
            try:
                for key in pck['change'][1]:
                    status[key] = pck['change'][1][key][1]
                    #status[key + '_prev'] = pck['change'][1][key][0]
            except:
                pass
    
    # copy autopilot dest on cfg save
    elif 'ui' in pck:
        try:
            status['autopilot_waypoints'] = pck['ui']['autopilot_waypoints'][1]
        except:
            pass
    

def get_status_str():
    return pformat(status)
