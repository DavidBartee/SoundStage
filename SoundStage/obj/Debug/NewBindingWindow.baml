   M S B A M L   `   `   `  ����E  ASoundStage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:SoundStage
SoundStage  5   	   !    SoundStage.NewBindingWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:SoundStage     5   	     � Title$  NewBindingWindow��5   	   $��406.667��6"   $��581.868��63   .��5	      �  � ColumnDefinitions 5
   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $	��55* �6   �� 5      $
��321* �6   .+�5   
   �� -   5         ��Name$ comboBoxBindType��5      $
��Left=�6-   $��
116,14,0,0q�6H   $	��Top=�6\   $	��130��6t   $��22��6�    �� SelectedIndex$ 0��6�   .@�5      �� $��Keyboard��6   �� 5      $��On-Screen Button��6   5      �� 5   
   $��Binding Type:��6   $
��Left=�6(   $��	10,10,0,0q�6C   $	��Top=�6V   $��35��6n   $	��101��6z   �� 5   
   -   5         ��Name$ textBoxBinding��5      $��1��6*   $
��Left=�6:   $��23��6U   $��	67,14,0,0q�6a    �� TextWrapping$
 Wrap=�6t   $	��Top=�6�   $	��170��6�    �� 
IsReadOnly . 6�   �� 5   
   -   5         ��Name$ 	labelKeys��5      $��Keys:��6#   $��1��63   $
��Left=�6C   $��	10,10,0,0q�6^   $	��Top=�6q   $��35��6�   $��52��6�   �� 5   
   -   5        	 ��Name$	 listBoxChooseSound��5      $	��251��6.   $��	10,10,0,0q�6;   $��1��6N   $	��554��6[   $��2��6g   
 �� SelectionMode$
 Extended=�6�   $
��Left=�6�   $	��Top=�6�   �� 5   
   -   5         ��Name$ btnCreateBinding��5      $��Create Binding��6+   $��1��6D   $
��Left=�6T   $��114,275,0,0q�6o   $��1��6�   $	��Top=�6�   $	��108��6�   $��36��6�   6�   5      5      