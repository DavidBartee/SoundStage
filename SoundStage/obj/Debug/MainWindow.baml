   M S B A M L   `   `   `  ����E  ASoundStage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:SoundStage
SoundStage  5   	       SoundStage.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:SoundStage     5   	     � Title$  Sound Stage��5   	   $	��450��6   $	��800��6*   
���   ����	#FF00E8FF � Icon$! images/Sound Stage icon.png��6`   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��264 �6   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $	��78* �6   �� 5      $
��341* �6   .+�5   
   �� -   5         ��Name$ listBoxSounds��5       J� 	AllowDrop . $	��100��6T   $��1��6d   �� 5   
   $��0q�6   $��2��6    $��2��61   $��1��6E   .@�5      �� $��Key Bindings��6   .��5      � .+�5      �� -   5         ��Name$ listViewKeyBinds��5      $��Center=�69    � ContextMenu 5      �� 5      .@�5   "   m� -   5        $
��Edit��5   +   69   m� 5   "   -   5        $��Delete��5   +   6;   5       �� View 5      �� 5      .P�5    "   �� $
��Keys��61   	 �� Width$		 200��6?   
 �� DisplayMemberBinding
 6K   �� *Keys+�� 5!   "   $��Sound��61   $		 200��6@   
 6L   �� *Sound+5"      5$      5%      5&      �� 5'      $��Buttons��6   .��5(      �� $
��Auto=�6   .��5)      � -   5         �Name$ wrapPanelSoundButtons��5)       $��1q�6?    � Orientation$ 
Horizontal=�6J   ��. $
��Left=�6�   $	��Top=�6�   5*      5+      �� 5,      $��Music Player��6   .��5-      �  5.      �� 5/      $	��107 �6(   �� 50       52      �� 53      $	��110 �6+   �� 54      .+�56      �� -   5         ��Name$ canvasCurrentSong��56      $��1��68   $
��Left=�6H   $��87��6c   $��10q�6o   $	��Top=�6{   $	��394��6�    J� RenderTransformOrigin$	 0,0&�6�   �� 57      -   5        $ listBoxQueuedSounds��57      $��1��6;   $��10q�6K   $��1��6W    . 6u   �� 58      -   5         ��Name$ btnPausePlay��58      $��10q�63   $��Disabled=�6?   
���   �.��59      �� -	   5         ��Name$ rectPausePlay��59   $   $��1q�6;   $��Disabled=�6F   $��60��6z   $��60��6�   
����� � J� OpacityMask 5:      �� 5;   "    �� ImageSource$ images/play.png��6-   5=      5>      5?      5@      5A      �� 5B   
   $��Center=�6   $��58��62   $��0q�6>   $��Center=�6I   $	��244��6d    �� Orientation$ 
Horizontal=�6p   .+�5C      �� -
   5        $ btnAddSound��5C      $��	Add Sound��6*   $��Center=�6>   $��0q�6[   $��Center=�6f   $��80��6�   $��36��6�   $��Center=�6�   �� 5D      -   5        $ btnNewBinding��5D      $��New Binding��6,   $��Center=�6B   $��0q�6_   $��Center=�6j   $��80��6�   $��36��6�   $��Center=�6�   6�   �� 5E      -   5        $ btnStopSounds��5E      $��Stop Sounds��6,   $��Center=�6B   $
��0,11q�6_   $��Center=�6m   $��80��6�   $��36��6�   $��Center=�6�   6�   5F      5H      5I      