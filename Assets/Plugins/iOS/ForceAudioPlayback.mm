#import <AVFoundation/AVFoundation.h>

extern "C" void _ForceAudioPlayback() {
    NSError *error = nil;
    AVAudioSession *session = [AVAudioSession sharedInstance];
    
    if (![session setCategory:AVAudioSessionCategoryPlayback error:&error]) {
        NSLog(@"Failed to set audio session category: %@", error.localizedDescription);
        return;
    }
    
    if (![session setActive:YES error:&error]) {
        NSLog(@"Failed to activate audio session: %@", error.localizedDescription);
    }
}
