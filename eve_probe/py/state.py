from pprint import pformat

status = {}

def on_packet(pck):
    
    if '__class__' in pck:
        # copy all session changes and prev vals
        if pck['__class__'].endswith('.SessionChangeNotification'):
            
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
    if 'ui' in pck:
        try:
            status['autopilot_waypoints'] = pck['ui']['autopilot_waypoints'][1]
        except:
            pass
    

def get_status_str():
    return pformat(status)
