import type { AuthService } from '@common/source/services/AuthService';
import { FolderApiClient } from './api/FolderApiClient';
import { MediaApiClient } from './api/MediaApiClient';
import { TagApiClient } from './api/TagApiClient';
import { UserApiClient } from './api/UserApiClient';
import { ConfigApiClient } from './api/ConfigApiClient';
import { SharedLinkApiClient } from './api/SharedLinkApiClient';
import { FrontApiClient } from './api/FrontApiClient';

export class ApiService {
  readonly folders: FolderApiClient;
  readonly media: MediaApiClient;
  readonly tags: TagApiClient;
  readonly users: UserApiClient;
  readonly config: ConfigApiClient;
  readonly sharedLinks: SharedLinkApiClient;
  readonly front: FrontApiClient;

  constructor($host: string, $auth: AuthService) {
    this.folders = new FolderApiClient($host, $auth);
    this.media = new MediaApiClient($host, $auth);
    this.tags = new TagApiClient($host, $auth);
    this.users = new UserApiClient($host, $auth);
    this.config = new ConfigApiClient($host, $auth);
    this.sharedLinks = new SharedLinkApiClient($host, $auth);
    this.front = new FrontApiClient($host, $auth);
  }
}
