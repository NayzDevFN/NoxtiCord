#include <windows.h>

static int file_exists(const wchar_t *path)
{
    DWORD a = GetFileAttributesW(path);
    return a != INVALID_FILE_ATTRIBUTES && !(a & FILE_ATTRIBUTE_DIRECTORY);
}

int WINAPI WinMain(HINSTANCE inst, HINSTANCE prev, LPSTR cmd, int show)
{
    wchar_t module[MAX_PATH];
    wchar_t dir[MAX_PATH];
    wchar_t target[MAX_PATH];
    wchar_t *slash;

    (void)inst;
    (void)prev;
    (void)cmd;
    (void)show;

    if (!GetModuleFileNameW(NULL, module, MAX_PATH))
    {
        MessageBoxW(NULL, L"Impossible de determiner l'emplacement de noxticord.exe",
                    L"Noxtcord", MB_OK | MB_ICONERROR);
        return 1;
    }

    wcscpy(dir, module);
    slash = wcsrchr(dir, L'\\');
    if (slash)
        *slash = L'\0';

    wcscpy(target, dir);
    wcscat(target, L"\\release\\NoxtiCord.exe");

    if (!file_exists(target))
    {
        MessageBoxW(NULL, L"release\\NoxtiCord.exe est introuvable.\n\n"
                          L"Reconstituez-le avec : npm run build:exe",
                    L"Noxtcord", MB_OK | MB_ICONERROR);
        return 1;
    }

    if ((INT_PTR)ShellExecuteW(NULL, L"open", target, NULL, dir, SW_SHOWNORMAL) <= 32)
    {
        MessageBoxW(NULL, L"Impossible de lancer release\\NoxtiCord.exe",
                    L"Noxtcord", MB_OK | MB_ICONERROR);
        return 1;
    }

    return 0;
}
