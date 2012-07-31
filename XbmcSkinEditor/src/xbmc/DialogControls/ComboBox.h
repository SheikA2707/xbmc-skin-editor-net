/*
This file is part of MultiClipboard Plugin for Notepad++
Copyright (C) 2009 LoonyChewy

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either
version 2 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/

#ifndef XBMC_COMBOBOX
#define XBMC_COMBOBOX


#ifndef UNITY_BUILD_SINGLE_INCLUDE
#include "Window.h"
#include "../lib/StdString.h"
#include <vector>
#endif


class CXBMCComboBox : public Window
{
public:
  virtual void init(HINSTANCE hInst, HWND parent);
  virtual void destroy();

  virtual void AddItem( CStdString item );
  virtual void ClearAll();
  virtual INT GetItemCount();

  virtual CStdString GetCurrentSelectionText();

  virtual INT GetCurrentSelectionIndex();
  // Selects the specified list box item.
  // If index out of bounds, and bStrictSelect is TRUE, nothing is selected
  // else the last item in the list, if available, is selected
  virtual void SetCurrentSelectedItem( INT NewSelectionIndex, BOOL bStrictSelect=TRUE );
  virtual void Resize(RECT rc);
private:
  std::vector<CStdString> lstText;
  HFONT hNewFont;

  WNDPROC oldWndProc;

  // Subclass the list box's wnd proc for customised behavior
  static LRESULT CALLBACK StaticListboxProc( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam )
  {
    return ((CXBMCComboBox *)(::GetWindowLongPtr(hwnd, GWL_USERDATA)))->runProc( hwnd, message, wParam, lParam );
  };
  LRESULT runProc( HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam );
};


#endif