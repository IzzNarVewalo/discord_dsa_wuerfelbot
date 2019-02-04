# discord_dsa_wuerfelbot
A bot which can roll trials from characters saved at https://online.helden-software.de.\
It uses [Discord.net](https://github.com/discord-net/Discord.Net), an unofficial .NET API Wrapper for the Discord client and needs access to a PostgreSQL Databse.
# Configuration
to set this bot up for your own usage create a token.cfg in the same directory as the binary.\
this token.cfg should contain the following data in exactly this sequence.

DISCORD_BOT_TOKEN\
ONLINE.HELDEN-SOFTWARE.DE_TOKEN\
IP_ADDRESS_FROM_POSTRESQL_SERVER\
POSTGRES_USERNAME\
POSTGRES_PASSWORD\
POSTGRES_TABLE

Replace this with your own informations.\
make sure only your user has permissions to this file since it contains sensitive data.
